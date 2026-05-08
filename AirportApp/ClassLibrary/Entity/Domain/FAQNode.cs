using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public record FAQNode(
        int faqNodeId,
        string questionText,
        ImmutableArray<FAQOption> options,
        bool isFinalAnswer);
}
