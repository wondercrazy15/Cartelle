using Stance.Models.Base;

namespace Stance.Models.Optimized
{
    public class OptContactAction : BaseAttributes
    {
        public bool Synced { get; set; }
        public OptAction Action { get; set; } // should just be the guid?
        public OptActionContent ActionContent { get; set; } // should just be the guid?
    }

    public class OptContactActionV2 : BaseAttributes
    {
        public bool Synced { get; set; }
        public OptActionV2 Action { get; set; } // should just be the guid?
        public OptActionContent ActionContent { get; set; } // should just be the guid?
    }
}
