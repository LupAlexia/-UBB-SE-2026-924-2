using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.Src.Dto;
using AirportApp.Src.Model.Faq;
using AirportApp.Src.Service.Implementation;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.View.Faq;

namespace AirportApp.Src.ViewModel.Faq
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
                ApplyFilters();
            }
        }

        public FAQCategoryEnum SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
                ApplyFilters();
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
            this.questionsService = faqService;
            this.mapper = faqMapper;

            frequentlyAskedQuestions = new ObservableCollection<FAQEntryDTO>();
            filteredQuestions = new ObservableCollection<FAQEntryDTO>();
            searchQuery = string.Empty;
            selectedCategory = FAQCategoryEnum.All;
        }

        public void LoadFAQ()
        {
            FAQs.Clear();

            var questionEntries = questionsService.GetAll().OrderByDescending(entry => entry.ViewCount);
            foreach (var entry in questionEntries)
            {
                FAQs.Add(mapper.Map<FAQEntryDTO>(entry));
            }

            ApplyFilters();
        }

        public void ApplyFilters()
        {
            var result = questionsService.FilterFAQEntry(SelectedCategory, SearchQuery)
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

        // public void Search()
        // {
        //    ApplyFilters();
        // }
        public void AddFAQEntry(FAQEntryDTO questionDataTransfer)
        {
            if (!IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can add FAQs.");
            }

            var questionEntity = mapper.Map<FAQEntry>(questionDataTransfer);
            questionsService.AddFAQEntry(questionEntity);
            LoadFAQ();
        }

        public void EditFAQEntry(FAQEntryDTO questionDataTransfer)
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
            questionsService.EditFAQEntry(questionEntity, questionDataTransfer.Id);
            LoadFAQ();
        }

        public void DeleteFAQEntry(FAQEntryDTO questionDataTransfer)
        {
            if (!IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can delete FAQs.");
            }

            if (questionDataTransfer == null)
            {
                throw new ArgumentNullException(nameof(questionDataTransfer));
            }

            questionsService.DeleteFAQEntry(questionDataTransfer.Id);
            LoadFAQ();
        }

        public void IncrementViewCount()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var questionEntity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            questionsService.IncrementViewCount(questionEntity);
            LoadFAQ();
        }
        public void IncrementWasHelpfulVotes()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var questionEntity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            questionsService.IncrementWasHelpfulVotes(questionEntity);

            SelectedFAQEntry.HelpfulVotesCount++;
            OnPropertyChanged(nameof(SelectedFAQEntry));
        }

        public void IncrementWasNotHelpfulVotes()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var questionEntity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            questionsService.IncrementWasNotHelpfulVotes(questionEntity);

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
                IncrementViewCountFor(questionDataTransfer.Id);
            }
            else
            {
                SelectedFAQEntry = null;
            }
        }

        public void IncrementViewCountFor(int questionId)
        {
            var frequentlyAskedQuestion = FAQs.FirstOrDefault(mainListDataTransfer => mainListDataTransfer.Id == questionId);
            if (frequentlyAskedQuestion == null)
            {
                return;
            }

            var questionEntity = mapper.Map<FAQEntry>(frequentlyAskedQuestion);
            questionsService.IncrementViewCount(questionEntity);

            frequentlyAskedQuestion.ViewCount++;

            var filteredFaq = FilteredFAQs.FirstOrDefault(filteredListDto => filteredListDto.Id == questionId);
            if (filteredFaq != null && filteredFaq != frequentlyAskedQuestion)
            {
                filteredFaq.ViewCount = frequentlyAskedQuestion.ViewCount;
            }

            OnPropertyChanged(nameof(FAQs));
            OnPropertyChanged(nameof(FilteredFAQs));
        }

        public Task Save(string question, string answer, string? categoryString)
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
                AddFAQEntry(sourceDataTransfer);
            }
            else
            {
                EditFAQEntry(sourceDataTransfer);
            }

            return Task.CompletedTask;
        }

        public void SetCategory(FAQCategoryEnum category)
        {
            SelectedCategory = category;
            ApplyFilters();
        }

        public void GiveFeedback(FAQEntryDTO frequentlyAskedQuestion, bool isHelpful)
        {
            if (frequentlyAskedQuestion == null)
            {
                return;
            }

            SelectedFAQEntry = frequentlyAskedQuestion;

            var questionEntity = mapper.Map<FAQEntry>(frequentlyAskedQuestion);

            if (isHelpful)
            {
                questionsService.IncrementWasHelpfulVotes(questionEntity);
                frequentlyAskedQuestion.HelpfulVotesCount++;
            }
            else
            {
                questionsService.IncrementWasNotHelpfulVotes(questionEntity);
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