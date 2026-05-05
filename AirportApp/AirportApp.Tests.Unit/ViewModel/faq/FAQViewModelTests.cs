using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.Src.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service.Implementation;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.ViewModel.Faq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Core;

namespace AirportApp.Tests.Unit.Src.ViewModel.Faq
{
    [TestClass]
    public class FAQViewModelTests
    {
        private IMapper _mapper;
        private IFAQService _faqService;
        private FAQViewModel _faqViewModel;

        [TestInitialize]
        public async Task Setup()
        {
            _mapper = Substitute.For<IMapper>();
            _faqService = Substitute.For<IFAQService>();

            _mapper.Map<FAQEntryDTO>(Arg.Any<FAQEntry>()).Returns(callInfo => MapToDto((FAQEntry)callInfo[0]));
            _mapper.Map<FAQEntry>(Arg.Any<FAQEntryDTO>()).Returns(callInfo => MapToEntity((FAQEntryDTO)callInfo[0]));

            var questionEntries = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };
            questionEntries = questionEntries.OrderByDescending(entry => entry.ViewCount).ToList();
            _faqService.GetAllAsync().Returns(Task.FromResult(questionEntries));
            _faqService.FilterFAQEntryAsync(Arg.Any<FAQCategoryEnum>(), Arg.Any<string>()).Returns(Task.FromResult(questionEntries));

            _faqViewModel = new FAQViewModel(_faqService, _mapper);
            await _faqViewModel.LoadFAQAsync();
        }

        [TestMethod]
        public async Task Constructor_WhenCalled_LoadsFAQsAndSetsVariable()
        {
            var allFrequentlyAskedQuestions = (await _faqService.GetAllAsync()).OrderByDescending(questionEntry => questionEntry.ViewCount).ToList();
            _faqService.FilterFAQEntryAsync(Arg.Any<FAQCategoryEnum>(), Arg.Any<string>()).Returns(Task.FromResult(allFrequentlyAskedQuestions));

            Assert.AreEqual(3, _faqViewModel.FAQs.Count);
            Assert.AreEqual(3, _faqViewModel.FilteredFAQs.Count);
            Assert.AreEqual(FAQCategoryEnum.All, _faqViewModel.SelectedCategory);
            Assert.AreEqual(string.Empty, _faqViewModel.SearchQuery);
        }

        [TestMethod]
        public void Search_ByQuestionOrAnswer_ReturnsCorrespondingFAQs()
        {
            var searchResults = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
            };
            searchResults = searchResults.OrderByDescending(entry => entry.ViewCount).ToList();
            _faqService.FilterFAQEntryAsync(FAQCategoryEnum.All, "park").Returns(Task.FromResult(searchResults));

            _faqViewModel.SearchQuery = "park";

            Assert.AreEqual(2, _faqViewModel.FilteredFAQs.Count);
            CollectionAssert.AreEqual(new[] { 2, 1 }, _faqViewModel.FilteredFAQs.Select(faqDto => faqDto.Id).ToArray());
        }

        [TestMethod]
        public void Filter_ByCategory_ReturnsCorrespondingFAQs()
        {
            var parkingEntries = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
            };
            parkingEntries = parkingEntries.OrderByDescending(entry => entry.ViewCount).ToList();
            _faqService.FilterFAQEntryAsync(FAQCategoryEnum.Parking, Arg.Any<string>()).Returns(Task.FromResult(parkingEntries));
            _faqViewModel.FilterByCategory(FAQCategoryEnum.Parking);

            Assert.AreEqual(FAQCategoryEnum.Parking, _faqViewModel.SelectedCategory);
            Assert.AreEqual(2, _faqViewModel.FilteredFAQs.Count);
            CollectionAssert.AreEqual(new[] { 2, 1 }, _faqViewModel.FilteredFAQs.Select(filteredFaq => filteredFaq.Id).ToArray());
        }

        [TestMethod]
        public async Task Delete_FAQEntryAsAdmin_Succeeds()
        {
            _faqViewModel.IsAdmin = true;
            var entryToDelete = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            var updatedEntries = new List<FAQEntry>
            {
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };
            _faqService.GetAllAsync().Returns(Task.FromResult(updatedEntries));
            var entryToDeleteDataTransferObject = MapToDto(entryToDelete);

            await _faqViewModel.DeleteFAQEntryAsync(entryToDeleteDataTransferObject);

            await _faqService.Received(1).DeleteFAQEntryAsync(entryToDelete.Id);
            Assert.AreEqual(2, _faqViewModel.FAQs.Count);
        }

        [TestMethod]
        public async Task Delete_FAQEntryNotAdmin_ThrowsUnauthorizedAccessException()
        {
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await _faqViewModel.DeleteFAQEntryAsync(MapToDto(new FAQEntry(4, "Q", "A", FAQCategoryEnum.Baggage, 0, 0, 0))));
            await _faqService.DidNotReceive().DeleteFAQEntryAsync(Arg.Any<int>());
        }

        [TestMethod]
        public async Task Delete_NullFAQEntry_ThrowsArgumentNullException()
        {
            _faqViewModel.IsAdmin = true;
            
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _faqViewModel.DeleteFAQEntryAsync(null));
            await _faqService.DidNotReceive().DeleteFAQEntryAsync(Arg.Any<int>());
        }

        [TestMethod]
        public void ToggleFAQ_WhenPressed_ExpandsEntryAndIncrementsViewCount()
        {
            var firstFrequentlyAskedQuestion = _faqViewModel.FilteredFAQs[0];
            var viewCountBeforeExpanding = firstFrequentlyAskedQuestion.ViewCount;
            var secondFrequentlyAskedQuestion = _faqViewModel.FilteredFAQs[1];

            _faqViewModel.ToggleFAQ(firstFrequentlyAskedQuestion);

            Assert.IsTrue(firstFrequentlyAskedQuestion.IsExpanded);
            Assert.IsFalse(secondFrequentlyAskedQuestion.IsExpanded);
            Assert.AreEqual(viewCountBeforeExpanding+1, _faqViewModel.FAQs.First(faqDataTransferObject => faqDataTransferObject.Id == firstFrequentlyAskedQuestion.Id).ViewCount);
            _faqService.Received(1).IncrementViewCountAsync(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void ToggleFAQ_CalledForNullEntity_ReturnsWithoutCallingService()
        {
            var firstFrequentlyAskedQuestion = _faqViewModel.FilteredFAQs[0];
            
            _faqViewModel.ToggleFAQ(null);

            Assert.IsFalse(firstFrequentlyAskedQuestion.IsExpanded);
            _faqService.DidNotReceive().IncrementViewCountAsync(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public async Task IncrementWasNotHelpfulVotes_ForSelectedFAQ_UpdatesNotHelpfulVotesCount()
        {
            var entryToIncrementNotHelpfulVotes = new FAQEntryDTO(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 4, 4, 2); ;
            _faqViewModel.SelectedFAQEntry = entryToIncrementNotHelpfulVotes;
            var expectedUpdatedEntry = new FAQEntryDTO(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 4, 4, 3);
           
            await _faqViewModel.IncrementWasNotHelpfulVotesAsync();

            await _faqService.Received(1).IncrementWasNotHelpfulVotesAsync(Arg.Any<FAQEntry>());
            Assert.AreEqual(expectedUpdatedEntry.NotHelpfulVotesCount, _faqViewModel.SelectedFAQEntry!.NotHelpfulVotesCount);
        }

        [TestMethod]
        public async Task Save_WithNewEntry_AddsFaq()
        {
            _faqViewModel.IsAdmin = true;

            await _faqViewModel.SaveAsync("Can my dog come on the plane?", "Depending on the breed", FAQCategoryEnum.Baggage.ToString());

            await _faqService.Received(1).AddFAQEntryAsync(Arg.Is<FAQEntry>(receivedEntry =>
                receivedEntry.Id == 0 &&
                receivedEntry.Question == "Can my dog come on the plane?" &&
                receivedEntry.Answer == "Depending on the breed" &&
                receivedEntry.Category == FAQCategoryEnum.Baggage));
        }

        [TestMethod]
        public async Task Save_WithExistingEntry_EditsFaq()
        {
            _faqViewModel.IsAdmin = true;
            _faqViewModel.SelectedFAQEntry = _faqViewModel.FAQs[0];

            await _faqViewModel.SaveAsync("Can my dog come on the plane?", "Depending on the size", FAQCategoryEnum.Parking.ToString());

            await _faqService.Received(1).EditFAQEntryAsync(
                Arg.Is<FAQEntry>(entityToUpdate =>
                    entityToUpdate.Id == _faqViewModel.FAQs[0].Id &&
                    entityToUpdate.Question == "Can my dog come on the plane?" &&
                    entityToUpdate.Answer == "Depending on the size" &&
                    entityToUpdate.Category == FAQCategoryEnum.Parking),
                _faqViewModel.FAQs[0].Id);
        }

        [TestMethod]
        public async Task Save_WithEmptyQuestion_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _faqViewModel.SaveAsync("   ", "Depending on the size", FAQCategoryEnum.Parking.ToString()));
        }

        [TestMethod]
        public async Task Save_WithEmptyAnswer_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _faqViewModel.SaveAsync("Can my dog come on the plane?", "   ", FAQCategoryEnum.Parking.ToString()));
        }

        [TestMethod]
        public async Task Save_WithInvalidCategory_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _faqViewModel.SaveAsync("Can my dog come on the plane?", "Depending on the size", "NotARealCategory"));
        }

        [TestMethod]
        public async Task GiveFeedback_Helpful_UpdatesFlagsAndVotes()
        {
            var frequentlyAskedQuestion = _faqViewModel.FilteredFAQs[0];
            var initialHelpfulVotes = frequentlyAskedQuestion.HelpfulVotesCount;

            await _faqViewModel.GiveFeedbackAsync(frequentlyAskedQuestion, true);

            await _faqService.Received(1).IncrementWasHelpfulVotesAsync(Arg.Any<FAQEntry>());
            Assert.AreEqual(initialHelpfulVotes + 1, frequentlyAskedQuestion.HelpfulVotesCount);
            Assert.IsTrue(frequentlyAskedQuestion.IsHelpfulSelected);
            Assert.IsFalse(frequentlyAskedQuestion.IsNotHelpfulSelected);
        }

        [TestMethod]
        public async Task GiveFeedback_NotHelpful_UpdatesFlagsAndVotes()
        {
            var frequentlyAskedQuestion = _faqViewModel.FilteredFAQs[0];
            var initialNotHelpfulVotes = frequentlyAskedQuestion.NotHelpfulVotesCount;

            await _faqViewModel.GiveFeedbackAsync(frequentlyAskedQuestion, false);

            await _faqService.Received(1).IncrementWasNotHelpfulVotesAsync(Arg.Any<FAQEntry>());
            Assert.AreEqual(initialNotHelpfulVotes + 1, frequentlyAskedQuestion.NotHelpfulVotesCount);
            Assert.IsTrue(frequentlyAskedQuestion.HasFeedback);
            Assert.IsTrue(frequentlyAskedQuestion.IsNotHelpfulSelected);
        }

        [TestMethod]
        public async Task GiveFeedback_WithNullFaq_DoesNothing()
        {
            await _faqViewModel.GiveFeedbackAsync(null, true);

            await _faqService.DidNotReceive().IncrementWasHelpfulVotesAsync(Arg.Any<FAQEntry>());
            await _faqService.DidNotReceive().IncrementWasNotHelpfulVotesAsync(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void BuildNavigationData_WhenCalled_IsSuccessful()
        {
            _faqViewModel.IsAdmin = true;
            _faqViewModel.SelectedFAQEntry = _faqViewModel.FAQs[1];

            var result = _faqViewModel.BuildNavigationData(42);

            Assert.AreEqual(42, result.CurrentPersonId);
            Assert.IsTrue(result.IsEmployee);
        }

        private static FAQEntryDTO MapToDto(FAQEntry questionEntry)
        {
            return new FAQEntryDTO(
                questionEntry.Id,
                questionEntry.Question,
                questionEntry.Answer,
                questionEntry.Category,
                questionEntry.ViewCount,
                questionEntry.HelpfulVotesCount,
                questionEntry.NotHelpfulVotesCount);
        }

        private static FAQEntry MapToEntity(FAQEntryDTO dataTransferObject)
        {
            return new FAQEntry(
                dataTransferObject.Id,
                dataTransferObject.Question,
                dataTransferObject.Answer,
                dataTransferObject.Category,
                dataTransferObject.ViewCount,
                dataTransferObject.HelpfulVotesCount,
                dataTransferObject.NotHelpfulVotesCount);
        }

        [TestMethod]
        public async Task IncrementViewCountFor_FaqNotFound_ReturnsWithoutCallingService()
        {
            var nonExistingId = 999;

            await _faqViewModel.IncrementViewCountForAsync(nonExistingId);

            await _faqService.DidNotReceive().IncrementViewCountAsync(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public async Task IncrementViewCountFor_FilteredFaqSameInstance_DoesNotDuplicateUpdate()
        {
            var frequentlyAskedQuestion = _faqViewModel.FAQs[0];
            var viewCountBeforeIncrementing = frequentlyAskedQuestion.ViewCount;
            _faqViewModel.FilteredFAQs.Clear();
            _faqViewModel.FilteredFAQs.Add(frequentlyAskedQuestion);

            await _faqViewModel.IncrementViewCountForAsync(frequentlyAskedQuestion.Id);

            await _faqService.Received(1).IncrementViewCountAsync(Arg.Any<FAQEntry>());
            Assert.AreEqual(viewCountBeforeIncrementing+1, frequentlyAskedQuestion.ViewCount);
        }

        [TestMethod]
        public void ToggleFAQ_WhenCollapsing_SetsSelectedToNull()
        {
            var frequentlyAskedQuestion = _faqViewModel.FilteredFAQs[0];

            _faqViewModel.ToggleFAQ(frequentlyAskedQuestion);
            Assert.IsTrue(frequentlyAskedQuestion.IsExpanded);

            _faqViewModel.ToggleFAQ(frequentlyAskedQuestion);

            Assert.IsFalse(frequentlyAskedQuestion.IsExpanded);
        }
    }
}
