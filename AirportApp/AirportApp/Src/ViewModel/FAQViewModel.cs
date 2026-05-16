using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Src.View.Faq;
using AirportApp.Src.View;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.ViewModel
{
    public class FAQViewModel : INotifyPropertyChanged
    {
        private readonly IFAQService questionsService;
        private readonly IMapper mapper;

        private ObservableCollection<FAQEntryDTO> frequentlyAskedQuestions;
        private ObservableCollection<FAQEntryDTO> filteredQuestions;
        private FAQEntryDTO? selectedFAQEntry;
        private string searchQuery;
        private FAQCategoryEnum selectedCategory;
        private bool isAdmin;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<FAQEntryDTO> FAQs
        {
            get => frequentlyAskedQuestions;
            set
            {
                frequentlyAskedQuestions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FAQEntryDTO> FilteredFAQs
        {
            get => filteredQuestions;
            set
            {
                filteredQuestions = value;
                OnPropertyChanged();
            }
        }

        public FAQEntryDTO? SelectedFAQEntry
        {
            get => selectedFAQEntry;
            set
            {
                selectedFAQEntry = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                OnPropertyChanged();
                _ = ApplyFiltersAsync();
            }
        }

        public FAQCategoryEnum SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
                _ = ApplyFiltersAsync();
            }
        }

        public bool IsAdmin
        {
            get => isAdmin;
            set
            {
                isAdmin = value;
                OnPropertyChanged();
            }
        }

        public FAQViewModel(IFAQService faqService, IMapper faqMapper)
        {
            questionsService = faqService;
            mapper = faqMapper;

            frequentlyAskedQuestions = new ObservableCollection<FAQEntryDTO>();
            filteredQuestions = new ObservableCollection<FAQEntryDTO>();
            searchQuery = string.Empty;
            selectedCategory = FAQCategoryEnum.All;
        }

        public async Task LoadFAQAsync()
        {
            FAQs.Clear();

            var questionEntries = (await questionsService.GetAllAsync()).OrderByDescending(entry => entry.ViewCount);
            foreach (var entry in questionEntries)
            {
                FAQs.Add(mapper.Map<FAQEntryDTO>(entry));
            }

            await ApplyFiltersAsync();
        }

        public async Task ApplyFiltersAsync()
        {
            var result = (await questionsService.FilterFAQEntryAsync(SelectedCategory, SearchQuery))
                                    .OrderByDescending(entry => entry.ViewCount)
                                    .AsEnumerable().Select(entry => mapper.Map<FAQEntryDTO>(entry));

            FilteredFAQs.Clear();
            foreach (var frequentlyAskedQuestion in result)
            {
                FilteredFAQs.Add(frequentlyAskedQuestion);
            }
        }

        public void FilterByCategory(FAQCategoryEnum category)
        {
            SelectedCategory = category;
        }

        public async Task AddFAQEntryAsync(FAQEntryDTO questionDataTransfer)
        {
            if (!IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can add FAQs.");
            }

            var questionEntity = mapper.Map<FAQEntry>(questionDataTransfer);
            await questionsService.AddFAQEntryAsync(questionEntity);
            await LoadFAQAsync();
        }

        public async Task EditFAQEntryAsync(FAQEntryDTO questionDataTransfer)
        {
            if (!IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can edit FAQs.");
            }

            if (questionDataTransfer == null)
            {
                throw new ArgumentNullException(nameof(questionDataTransfer));
            }

            var questionEntity = mapper.Map<FAQEntry>(questionDataTransfer);
            await questionsService.EditFAQEntryAsync(questionEntity, questionDataTransfer.Id);
            await LoadFAQAsync();
        }

        public async Task DeleteFAQEntryAsync(FAQEntryDTO questionDataTransfer)
        {
            if (!IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can delete FAQs.");
            }

            if (questionDataTransfer == null)
            {
                throw new ArgumentNullException(nameof(questionDataTransfer));
            }

            await questionsService.DeleteFAQEntryAsync(questionDataTransfer.Id);
            await LoadFAQAsync();
        }

        public async Task IncrementViewCountAsync()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var questionEntity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            await questionsService.IncrementViewCountAsync(questionEntity);
            await LoadFAQAsync();
        }

        public async Task IncrementWasHelpfulVotesAsync()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var questionEntity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            await questionsService.IncrementWasHelpfulVotesAsync(questionEntity);

            SelectedFAQEntry.HelpfulVotesCount++;
            OnPropertyChanged(nameof(SelectedFAQEntry));
        }

        public async Task IncrementWasNotHelpfulVotesAsync()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var questionEntity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            await questionsService.IncrementWasNotHelpfulVotesAsync(questionEntity);

            SelectedFAQEntry.NotHelpfulVotesCount++;
            OnPropertyChanged(nameof(SelectedFAQEntry));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ToggleFAQ(FAQEntryDTO questionDataTransfer)
        {
            if (questionDataTransfer == null)
            {
                return;
            }

            bool willExpand = !questionDataTransfer.IsExpanded;

            foreach (var frequentlyAskedQuestion in FilteredFAQs)
            {
                frequentlyAskedQuestion.IsExpanded = false;
            }

            questionDataTransfer.IsExpanded = willExpand;

            if (willExpand)
            {
                SelectedFAQEntry = questionDataTransfer;
                _ = IncrementViewCountForAsync(questionDataTransfer.Id);
            }
            else
            {
                SelectedFAQEntry = null;
            }
        }

        public async Task IncrementViewCountForAsync(int questionId)
        {
            var frequentlyAskedQuestion = FAQs.FirstOrDefault(mainListDataTransfer => mainListDataTransfer.Id == questionId);
            if (frequentlyAskedQuestion == null)
            {
                return;
            }

            var questionEntity = mapper.Map<FAQEntry>(frequentlyAskedQuestion);
            await questionsService.IncrementViewCountAsync(questionEntity);

            frequentlyAskedQuestion.ViewCount++;

            var filteredFaq = FilteredFAQs.FirstOrDefault(filteredListDto => filteredListDto.Id == questionId);
            if (filteredFaq != null && filteredFaq != frequentlyAskedQuestion)
            {
                filteredFaq.ViewCount = frequentlyAskedQuestion.ViewCount;
            }

            OnPropertyChanged(nameof(FAQs));
            OnPropertyChanged(nameof(FilteredFAQs));
        }

        public async Task SaveAsync(string question, string answer, string? categoryString)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new ArgumentException("Answer cannot be empty.");
            }

            if (!Enum.TryParse<FAQCategoryEnum>(categoryString, out var category))
            {
                throw new ArgumentException("Invalid category.");
            }

            var sourceDataTransfer = new FAQEntryDTO(
                SelectedFAQEntry?.Id ?? 0,
                question.Trim(),
                answer.Trim(),
                category,
                SelectedFAQEntry?.ViewCount ?? 0,
                SelectedFAQEntry?.HelpfulVotesCount ?? 0,
                SelectedFAQEntry?.NotHelpfulVotesCount ?? 0);

            if (sourceDataTransfer.Id == 0)
            {
                await AddFAQEntryAsync(sourceDataTransfer);
            }
            else
            {
                await EditFAQEntryAsync(sourceDataTransfer);
            }
        }

        public void SetCategory(FAQCategoryEnum category)
        {
            SelectedCategory = category;
            _ = ApplyFiltersAsync();
        }

        public async Task GiveFeedbackAsync(FAQEntryDTO frequentlyAskedQuestion, bool isHelpful)
        {
            if (frequentlyAskedQuestion == null)
            {
                return;
            }

            SelectedFAQEntry = frequentlyAskedQuestion;

            var questionEntity = mapper.Map<FAQEntry>(frequentlyAskedQuestion);

            if (isHelpful)
            {
                await questionsService.IncrementWasHelpfulVotesAsync(questionEntity);
                frequentlyAskedQuestion.HelpfulVotesCount++;
            }
            else
            {
                await questionsService.IncrementWasNotHelpfulVotesAsync(questionEntity);
                frequentlyAskedQuestion.NotHelpfulVotesCount++;
            }

            frequentlyAskedQuestion.HasFeedback = true;
            frequentlyAskedQuestion.IsHelpfulSelected = isHelpful;
            frequentlyAskedQuestion.IsNotHelpfulSelected = !isHelpful;

            OnPropertyChanged(nameof(SelectedFAQEntry));
        }

        public FAQNavigationData BuildNavigationData(int currentPersonId)
        {
            return new FAQNavigationData
            {
                CurrentPersonId = currentPersonId,
                IsEmployee = IsAdmin,
                FAQEntry = SelectedFAQEntry
            };
        }
    }
}