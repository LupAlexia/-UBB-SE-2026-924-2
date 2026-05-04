using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Service.Implementation;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.ViewModel.Faq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Core;

namespace CloudSpritzers1Tests.Src.ViewModel.Faq
{
    [TestClass]
    public class FAQViewModelTests
    {
        private IMapper _mapper;
        private IFAQService _faqService;
        private FAQViewModel _faqViewModel;

        [TestInitialize]
        public void Setup()
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
            _faqService.GetAll().Returns(questionEntries);
            _faqService.FilterFAQEntry(Arg.Any<FAQCategoryEnum>(), Arg.Any<string>()).Returns(questionEntries);

            _faqViewModel = new FAQViewModel(_faqService, _mapper);
            _faqViewModel.LoadFAQ();
        }

        [TestMethod]
        public void Constructor_WhenCalled_LoadsFAQsAndSetsVariable()
        {
            var allFrequentlyAskedQuestions = _faqService.GetAll().OrderByDescending(questionEntry => questionEntry.ViewCount).ToList();
            _faqService.FilterFAQEntry(Arg.Any<FAQCategoryEnum>(), Arg.Any<string>()).Returns(allFrequentlyAskedQuestions);

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
            _faqService.FilterFAQEntry(FAQCategoryEnum.All, "park").Returns(searchResults);

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
            parkingEntries.OrderByDescending(entry => entry.ViewCount).ToList();
            _faqService.FilterFAQEntry(FAQCategoryEnum.Parking, Arg.Any<string>()).Returns(parkingEntries);
            _faqViewModel.FilterByCategory(FAQCategoryEnum.Parking);

            Assert.AreEqual(FAQCategoryEnum.Parking, _faqViewModel.SelectedCategory);
            Assert.AreEqual(2, _faqViewModel.FilteredFAQs.Count);
            CollectionAssert.AreEqual(new[] { 2, 1 }, _faqViewModel.FilteredFAQs.Select(filteredFaq => filteredFaq.Id).ToArray());
        }

        [TestMethod]
        public void Delete_FAQEntryAsAdmin_Succeeds()
        {
            _faqViewModel.IsAdmin = true;
            var entryToDelete = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            var updatedEntries = new List<FAQEntry>
            {
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };
            _faqService.GetAll().Returns(updatedEntries);
            var entryToDeleteDataTransferObject = MapToDto(entryToDelete);

            _faqViewModel.DeleteFAQEntry(entryToDeleteDataTransferObject);

            _faqService.Received(1).DeleteFAQEntry(entryToDelete.Id);
            Assert.AreEqual(2, _faqViewModel.FAQs.Count);
        }

        [TestMethod]
        public void Delete_FAQEntryNotAdmin_ThrowsUnauthorizedAccessException()
        {
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => _faqViewModel.DeleteFAQEntry(MapToDto(new FAQEntry(4, "Q", "A", FAQCategoryEnum.Baggage, 0, 0, 0))));
            _faqService.DidNotReceive().DeleteFAQEntry(Arg.Any<int>());
        }

        [TestMethod]
        public void Delete_NullFAQEntry_ThrowsArgumentNullException()
        {
            _faqViewModel.IsAdmin = true;
            
            Assert.ThrowsExactly<ArgumentNullException>(() => _faqViewModel.DeleteFAQEntry(null));
            _faqService.DidNotReceive().DeleteFAQEntry(Arg.Any<int>());
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
            _faqService.Received(1).IncrementViewCount(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void ToggleFAQ_CalledForNullEntity_ReturnsWithoutCallingService()
        {
            var firstFrequentlyAskedQuestion = _faqViewModel.FilteredFAQs[0];
            
            _faqViewModel.ToggleFAQ(null);

            Assert.IsFalse(firstFrequentlyAskedQuestion.IsExpanded);
            _faqService.DidNotReceive().IncrementViewCount(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void IncrementWasNotHelpfulVotes_ForSelectedFAQ_UpdatesNotHelpfulVotesCount()
        {
            var entryToIncrementNotHelpfulVotes = new FAQEntryDTO(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 4, 4, 2); ;
            _faqViewModel.SelectedFAQEntry = entryToIncrementNotHelpfulVotes;
            var expectedUpdatedEntry = new FAQEntryDTO(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 4, 4, 3);
           
            _faqViewModel.IncrementWasNotHelpfulVotes();

            _faqService.Received(1).IncrementWasNotHelpfulVotes(Arg.Any<FAQEntry>());
            Assert.AreEqual(expectedUpdatedEntry.NotHelpfulVotesCount, _faqViewModel.SelectedFAQEntry!.NotHelpfulVotesCount);
        }

        [TestMethod]
        public async Task Save_WithNewEntry_AddsFaq()
        {
            _faqViewModel.IsAdmin = true;

            await _faqViewModel.Save("Can my dog come on the plane?", "Depending on the breed", FAQCategoryEnum.Baggage.ToString());

            _faqService.Received(1).AddFAQEntry(Arg.Is<FAQEntry>(receivedEntry =>
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

            await _faqViewModel.Save("Can my dog come on the plane?", "Depending on the size", FAQCategoryEnum.Parking.ToString());

            _faqService.Received(1).EditFAQEntry(
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
            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
                await _faqViewModel.Save("   ", "Depending on the size", FAQCategoryEnum.Parking.ToString()));
        }

        [TestMethod]
        public async Task Save_WithEmptyAnswer_ThrowsArgumentException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
                await _faqViewModel.Save("Can my dog come on the plane?", "   ", FAQCategoryEnum.Parking.ToString()));
        }

        [TestMethod]
        public async Task Save_WithInvalidCategory_ThrowsArgumentException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
                await _faqViewModel.Save("Can my dog come on the plane?", "Depending on the size", "NotARealCategory"));
        }

        [TestMethod]
        public void GiveFeedback_Helpful_UpdatesFlagsAndVotes()
        {
            var frequentlyAskedQuestion = _faqViewModel.FilteredFAQs[0];
            var initialHelpfulVotes = frequentlyAskedQuestion.HelpfulVotesCount;

            _faqViewModel.GiveFeedback(frequentlyAskedQuestion, true);

            _faqService.Received(1).IncrementWasHelpfulVotes(Arg.Any<FAQEntry>());
            Assert.AreEqual(initialHelpfulVotes + 1, frequentlyAskedQuestion.HelpfulVotesCount);
            Assert.IsTrue(frequentlyAskedQuestion.IsHelpfulSelected);
            Assert.IsFalse(frequentlyAskedQuestion.IsNotHelpfulSelected);
        }

        [TestMethod]
        public void GiveFeedback_NotHelpful_UpdatesFlagsAndVotes()
        {
            var frequentlyAskedQuestion = _faqViewModel.FilteredFAQs[0];
            var initialNotHelpfulVotes = frequentlyAskedQuestion.NotHelpfulVotesCount;

            _faqViewModel.GiveFeedback(frequentlyAskedQuestion, false);

            _faqService.Received(1).IncrementWasNotHelpfulVotes(Arg.Any<FAQEntry>());
            Assert.AreEqual(initialNotHelpfulVotes + 1, frequentlyAskedQuestion.NotHelpfulVotesCount);
            Assert.IsTrue(frequentlyAskedQuestion.HasFeedback);
            Assert.IsTrue(frequentlyAskedQuestion.IsNotHelpfulSelected);
        }

        [TestMethod]
        public void GiveFeedback_WithNullFaq_DoesNothing()
        {
            _faqViewModel.GiveFeedback(null, true);

            _faqService.DidNotReceive().IncrementWasHelpfulVotes(Arg.Any<FAQEntry>());
            _faqService.DidNotReceive().IncrementWasNotHelpfulVotes(Arg.Any<FAQEntry>());
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
        public void IncrementViewCountFor_FaqNotFound_ReturnsWithoutCallingService()
        {
            var nonExistingId = 999;

            _faqViewModel.IncrementViewCountFor(nonExistingId);

            _faqService.DidNotReceive().IncrementViewCount(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void IncrementViewCountFor_FilteredFaqSameInstance_DoesNotDuplicateUpdate()
        {
            var frequentlyAskedQuestion = _faqViewModel.FAQs[0];
            var viewCountBeforeIncrementing = frequentlyAskedQuestion.ViewCount;
            _faqViewModel.FilteredFAQs.Clear();
            _faqViewModel.FilteredFAQs.Add(frequentlyAskedQuestion);

            _faqViewModel.IncrementViewCountFor(frequentlyAskedQuestion.Id);

            _faqService.Received(1).IncrementViewCount(Arg.Any<FAQEntry>());
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
