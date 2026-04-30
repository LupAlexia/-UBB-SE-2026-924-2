using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Service.Bot;

namespace AirportApp.Src.Model.Faq.Bot
{
    public record FAQNode(
        int faqNodeId,
        string questionText,
        ImmutableArray<FAQOption> options,
        bool isFinalAnswer);
}
