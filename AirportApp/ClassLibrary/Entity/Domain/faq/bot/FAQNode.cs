using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;

namespace AirportApp.ClassLibrary.Entity.Domain.Faq.Bot
{
    public record FAQNode(
        int FaqNodeId,
        string QuestionText,
        ImmutableArray<FAQOption> Options,
        bool IsFinalAnswer);
}
