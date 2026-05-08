using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service.Implementation;
using AirportApp.Src.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Core;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Src.ViewModel.Faq
{
    [TestClass]
    public class FAQViewModelTests
    {
        private IMapper mapper;
        private IFAQService faqService;
        private FAQViewModel faqViewModel;

        [TestInitialize]
        public async Task Setup()
        {
            mapper = Substitute.For<IMapper>();
            faqService = Substitute.For<IFAQService>();

            mapper.Map<FAQEntryDTO>(Arg.Any<FAQEntry>()).Returns(callInfo => MapToDto((FAQEntry)callInfo[0]));
            mapper.Map<FAQEntry>(Arg.Any<FAQEntryDTO>()).Returns(callInfo => MapToEntity((FAQEntryDTO)callInfo[0]));

            var questionEntries = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };
            questionEntries = questionEntries.OrderByDescending(entry => entry.ViewCount).ToList();
            faqService.GetAllAsync().Returns(Task.FromResult(questionEntries));
            faqService.FilterFAQEntryAsync(Arg.Any<FAQCategoryEnum>(), Arg.Any<string>()).Returns(Task.FromResult(questionEntries));

            faqViewModel = new FAQViewModel(faqService, mapper);
            await faqViewModel.LoadFAQAsync();
        }

        [TestMethod]
        public async Task Constructor_WhenCalled_LoadsFAQsAndSetsVariable()
        {
            var allFrequentlyAskedQuestions = (await faqService.GetAllAsync()).OrderByDescending(questionEntry => questionEntry.ViewCount).ToList();
            faqService.FilterFAQEntryAsync(Arg.Any<FAQCategoryEnum>(), Arg.Any<string>()).Returns(Task.FromResult(allFrequentlyAskedQuestions));

            Assert.AreEqual(3, faqViewModel.FAQs.Count);
            Assert.AreEqual(3, faqViewModel.FilteredFAQs.Count);
            Assert.AreEqual(FAQCategoryEnum.All, faqViewModel.SelectedCategory);
            Assert.AreEqual(string.Empty, faqViewModel.SearchQuery);
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
            faqService.FilterFAQEntryAsync(FAQCategoryEnum.All, "park").Returns(Task.FromResult(searchResults));

            faqViewModel.SearchQuery = "park";

            Assert.AreEqual(2, faqViewModel.FilteredFAQs.Count);
            CollectionAssert.AreEqual(new[] { 2, 1 }, faqViewModel.FilteredFAQs.Select(faqDto => faqDto.Id).ToArray());
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
            faqService.FilterFAQEntryAsync(FAQCategoryEnum.Parking, Arg.Any<string>()).Returns(Task.FromResult(parkingEntries));
            faqViewModel.FilterByCategory(FAQCategoryEnum.Parking);

            Assert.AreEqual(FAQCategoryEnum.Parking, faqViewModel.SelectedCategory);
            Assert.AreEqual(2, faqViewModel.FilteredFAQs.Count);
            CollectionAssert.AreEqual(new[] { 2, 1 }, faqViewModel.FilteredFAQs.Select(filteredFaq => filteredFaq.Id).ToArray());
        }

        [TestMethod]
        public async Task Delete_FAQEntryAsAdmin_Succeeds()
        {
            faqViewModel.IsAdmin = true;
            var entryToDelete = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            var updatedEntries = new List<FAQEntry>
            {
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };
            faqService.GetAllAsync().Returns(Task.FromResult(updatedEntries));
            var entryToDeleteDataTransferObject = MapToDto(entryToDelete);

            await faqViewModel.DeleteFAQEntryAsync(entryToDeleteDataTransferObject);

            await faqService.Received(1).DeleteFAQEntryAsync(entryToDelete.Id);
            Assert.AreEqual(2, faqViewModel.FAQs.Count);
        }

        [TestMethod]
        public async Task Delete_FAQEntryNotAdmin_ThrowsUnauthorizedAccessException()
        {
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await faqViewModel.DeleteFAQEntryAsync(MapToDto(new FAQEntry(4, "Q", "A", FAQCategoryEnum.Baggage, 0, 0, 0))));
            await faqService.DidNotReceive().DeleteFAQEntryAsync(Arg.Any<int>());
        }

        [TestMethod]
        public async Task Delete_NullFAQEntry_ThrowsArgumentNullException()
        {
            faqViewModel.IsAdmin = true;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await faqViewModel.DeleteFAQEntryAsync(null));
            await faqService.DidNotReceive().DeleteFAQEntryAsync(Arg.Any<int>());
        }

        [TestMethod]
        public void ToggleFAQ_WhenPressed_ExpandsEntryAndIncrementsViewCount()
        {
            var firstFrequentlyAskedQuestion = faqViewModel.FilteredFAQs[0];
            var viewCountBeforeExpanding = firstFrequentlyAskedQuestion.ViewCount;
            var secondFrequentlyAskedQuestion = faqViewModel.FilteredFAQs[1];

            faqViewModel.ToggleFAQ(firstFrequentlyAskedQuestion);

            Assert.IsTrue(firstFrequentlyAskedQuestion.IsExpanded);
            Assert.IsFalse(secondFrequentlyAskedQuestion.IsExpanded);
            Assert.AreEqual(viewCountBeforeExpanding + 1, faqViewModel.FAQs.First(faqDataTransferObject => faqDataTransferObject.Id == firstFrequentlyAskedQuestion.Id).ViewCount);
            faqService.Received(1).IncrementViewCountAsync(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void ToggleFAQ_CalledForNullEntity_ReturnsWithoutCallingService()
        {
            var firstFrequentlyAskedQuestion = faqViewModel.FilteredFAQs[0];

            faqViewModel.ToggleFAQ(null);

            Assert.IsFalse(firstFrequentlyAskedQuestion.IsExpanded);
            faqService.DidNotReceive().IncrementViewCountAsync(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public async Task IncrementWasNotHelpfulVotes_ForSelectedFAQ_UpdatesNotHelpfulVotesCount()
        {
            var entryToIncrementNotHelpfulVotes = new FAQEntryDTO(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 4, 4, 2);
            faqViewModel.SelectedFAQEntry = entryToIncrementNotHelpfulVotes;
            var expectedUpdatedEntry = new FAQEntryDTO(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 4, 4, 3);

            await faqViewModel.IncrementWasNotHelpfulVotesAsync();

            await faqService.Received(1).IncrementWasNotHelpfulVotesAsync(Arg.Any<FAQEntry>());
            Assert.AreEqual(expectedUpdatedEntry.NotHelpfulVotesCount, faqViewModel.SelectedFAQEntry!.NotHelpfulVotesCount);
        }

        [TestMethod]
        public async Task Save_WithNewEntry_AddsFaq()
        {
            faqViewModel.IsAdmin = true;

            await faqViewModel.SaveAsync("Can my dog come on the plane?", "Depending on the breed", FAQCategoryEnum.Baggage.ToString());

            await faqService.Received(1).AddFAQEntryAsync(Arg.Is<FAQEntry>(receivedEntry =>
                receivedEntry.Id == 0 &&
                receivedEntry.Question == "Can my dog come on the plane?" &&
                receivedEntry.Answer == "Depending on the breed" &&
                receivedEntry.Category == FAQCategoryEnum.Baggage));
        }

        [TestMethod]
        public async Task Save_WithExistingEntry_EditsFaq()
        {
            faqViewModel.IsAdmin = true;
            faqViewModel.SelectedFAQEntry = faqViewModel.FAQs[0];

            await faqViewModel.SaveAsync("Can my dog come on the plane?", "Depending on the size", FAQCategoryEnum.Parking.ToString());

            await faqService.Received(1).EditFAQEntryAsync(
                Arg.Is<FAQEntry>(entityToUpdate =>
                    entityToUpdate.Id == faqViewModel.FAQs[0].Id &&
                    entityToUpdate.Question == "Can my dog come on the plane?" &&
                    entityToUpdate.Answer == "Depending on the size" &&
                    entityToUpdate.Category == FAQCategoryEnum.Parking),
                faqViewModel.FAQs[0].Id);
        }

        [TestMethod]
        public async Task Save_WithEmptyQuestion_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await faqViewModel.SaveAsync("   ", "Depending on the size", FAQCategoryEnum.Parking.ToString()));
        }

        [TestMethod]
        public async Task Save_WithEmptyAnswer_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await faqViewModel.SaveAsync("Can my dog come on the plane?", "   ", FAQCategoryEnum.Parking.ToString()));
        }

        [TestMethod]
        public async Task Save_WithInvalidCategory_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await faqViewModel.SaveAsync("Can my dog come on the plane?", "Depending on the size", "NotARealCategory"));
        }

        [TestMethod]
        public async Task GiveFeedback_Helpful_UpdatesFlagsAndVotes()
        {
            var frequentlyAskedQuestion = faqViewModel.FilteredFAQs[0];
            var initialHelpfulVotes = frequentlyAskedQuestion.HelpfulVotesCount;

            await faqViewModel.GiveFeedbackAsync(frequentlyAskedQuestion, true);

            await faqService.Received(1).IncrementWasHelpfulVotesAsync(Arg.Any<FAQEntry>());
            Assert.AreEqual(initialHelpfulVotes + 1, frequentlyAskedQuestion.HelpfulVotesCount);
            Assert.IsTrue(frequentlyAskedQuestion.IsHelpfulSelected);
            Assert.IsFalse(frequentlyAskedQuestion.IsNotHelpfulSelected);
        }

        [TestMethod]
        public async Task GiveFeedback_NotHelpful_UpdatesFlagsAndVotes()
        {
            var frequentlyAskedQuestion = faqViewModel.FilteredFAQs[0];
            var initialNotHelpfulVotes = frequentlyAskedQuestion.NotHelpfulVotesCount;

            await faqViewModel.GiveFeedbackAsync(frequentlyAskedQuestion, false);

            await faqService.Received(1).IncrementWasNotHelpfulVotesAsync(Arg.Any<FAQEntry>());
            Assert.AreEqual(initialNotHelpfulVotes + 1, frequentlyAskedQuestion.NotHelpfulVotesCount);
            Assert.IsTrue(frequentlyAskedQuestion.HasFeedback);
            Assert.IsTrue(frequentlyAskedQuestion.IsNotHelpfulSelected);
        }

        [TestMethod]
        public async Task GiveFeedback_WithNullFaq_DoesNothing()
        {
            await faqViewModel.GiveFeedbackAsync(null, true);

            await faqService.DidNotReceive().IncrementWasHelpfulVotesAsync(Arg.Any<FAQEntry>());
            await faqService.DidNotReceive().IncrementWasNotHelpfulVotesAsync(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void BuildNavigationData_WhenCalled_IsSuccessful()
        {
            faqViewModel.IsAdmin = true;
            faqViewModel.SelectedFAQEntry = faqViewModel.FAQs[1];

            var result = faqViewModel.BuildNavigationData(42);

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

            await faqViewModel.IncrementViewCountForAsync(nonExistingId);

            await faqService.DidNotReceive().IncrementViewCountAsync(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public async Task IncrementViewCountFor_FilteredFaqSameInstance_DoesNotDuplicateUpdate()
        {
            var frequentlyAskedQuestion = faqViewModel.FAQs[0];
            var viewCountBeforeIncrementing = frequentlyAskedQuestion.ViewCount;
            faqViewModel.FilteredFAQs.Clear();
            faqViewModel.FilteredFAQs.Add(frequentlyAskedQuestion);

            await faqViewModel.IncrementViewCountForAsync(frequentlyAskedQuestion.Id);

            await faqService.Received(1).IncrementViewCountAsync(Arg.Any<FAQEntry>());
            Assert.AreEqual(viewCountBeforeIncrementing + 1, frequentlyAskedQuestion.ViewCount);
        }

        [TestMethod]
        public void ToggleFAQ_WhenCollapsing_SetsSelectedToNull()
        {
            var frequentlyAskedQuestion = faqViewModel.FilteredFAQs[0];

            faqViewModel.ToggleFAQ(frequentlyAskedQuestion);
            Assert.IsTrue(frequentlyAskedQuestion.IsExpanded);

            faqViewModel.ToggleFAQ(frequentlyAskedQuestion);

            Assert.IsFalse(frequentlyAskedQuestion.IsExpanded);
        }
    }
}
