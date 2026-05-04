using AutoMapper;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudSpritzers1Tests.Src.Dto.MappingProfiles;
[TestClass]
public class FAQEntryMappingProfileTests
{
    private IMapper _mapper;
    private FAQEntry _frequentlyAskedQuestionsEntry;
    private FAQEntryDTO _frequentlyAskedQuestionsDataTransferObject;

    [TestInitialize]
    public void Setup()
    {
        var configuration = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile<FAQEntryMappingProfile>());
        _mapper = configuration.CreateMapper();
        _frequentlyAskedQuestionsEntry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
        _frequentlyAskedQuestionsDataTransferObject = new FAQEntryDTO(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
    }

    [TestMethod]
    public void Map_FromEntryToDTO_MapsIdCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<FAQEntryDTO>(_frequentlyAskedQuestionsEntry);

        Assert.AreEqual(_frequentlyAskedQuestionsDataTransferObject.Id, resultedDataTransferObject.Id);
    }
    [TestMethod]
    public void Map_FromEntryToDTO_MapsQuestionCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<FAQEntryDTO>(_frequentlyAskedQuestionsEntry);

        Assert.AreEqual(_frequentlyAskedQuestionsDataTransferObject.Question, resultedDataTransferObject.Question);
    }

    [TestMethod]
    public void Map_FromEntryToDTO_MapsAnswerCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<FAQEntryDTO>(_frequentlyAskedQuestionsEntry);

        Assert.AreEqual(_frequentlyAskedQuestionsDataTransferObject.Answer, resultedDataTransferObject.Answer); 
    }

    [TestMethod]
    public void Map_FromEntryToDTO_MapsViewCountCorrectly() 
    {
        var resultedDataTransferObject = _mapper.Map<FAQEntryDTO>(_frequentlyAskedQuestionsEntry);

        Assert.AreEqual(_frequentlyAskedQuestionsDataTransferObject.ViewCount, resultedDataTransferObject.ViewCount); 
    }

    [TestMethod]
    public void Map_FromEntryToDTO_MapsHelpfulVotesCountCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<FAQEntryDTO>(_frequentlyAskedQuestionsEntry);

        Assert.AreEqual(_frequentlyAskedQuestionsDataTransferObject.HelpfulVotesCount, resultedDataTransferObject.HelpfulVotesCount);
    }

    [TestMethod]
    public void Map_FromEntryToDTO_MapsNotHelpfulVotesCountCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<FAQEntryDTO>(_frequentlyAskedQuestionsEntry);

        Assert.AreEqual(_frequentlyAskedQuestionsDataTransferObject.NotHelpfulVotesCount, resultedDataTransferObject.NotHelpfulVotesCount);
    }

    [TestMethod]
    public void Map_FromDtoToEntry_MapsIdCorrectly()
    {
        var resultedEntity = _mapper.Map<FAQEntry>(_frequentlyAskedQuestionsDataTransferObject);

        Assert.AreEqual(_frequentlyAskedQuestionsEntry.Id, resultedEntity.Id);
    }

    [TestMethod]
    public void Map_FromDtoToEntry_MapsQuestionCorrectly()
    {
        var resultedEntity = _mapper.Map<FAQEntry>(_frequentlyAskedQuestionsDataTransferObject);

        Assert.AreEqual(_frequentlyAskedQuestionsEntry.Question, resultedEntity.Question);
    }

    [TestMethod]
    public void Map_FromDtoToEntry_MapsAnswerCorrectly()
    {
        var resultedEntity = _mapper.Map<FAQEntry>(_frequentlyAskedQuestionsDataTransferObject);

        Assert.AreEqual(_frequentlyAskedQuestionsEntry.Answer, resultedEntity.Answer);
    }

    [TestMethod]
    public void Map_FromDtoToEntry_MapsViewCountCorrectly()
    {
        var resultedEntity = _mapper.Map<FAQEntry>(_frequentlyAskedQuestionsDataTransferObject);

        Assert.AreEqual(_frequentlyAskedQuestionsEntry.ViewCount, resultedEntity.ViewCount);
    }

    [TestMethod]
    public void Map_FromDtoToEntry_MapsHelpfulVotesCountCorrectly()
    {
        var resultedEntity = _mapper.Map<FAQEntry>(_frequentlyAskedQuestionsDataTransferObject);

        Assert.AreEqual(_frequentlyAskedQuestionsEntry.HelpfulVotesCount, resultedEntity.HelpfulVotesCount);
    }

    [TestMethod]
    public void Map_FromDtoToEntry_MapsNotHelpfulVotesCountCorrectly()
    {
        var resultedEntity = _mapper.Map<FAQEntry>(_frequentlyAskedQuestionsDataTransferObject);

        Assert.AreEqual(_frequentlyAskedQuestionsEntry.NotHelpfulVotesCount, resultedEntity.NotHelpfulVotesCount);
    }
}
