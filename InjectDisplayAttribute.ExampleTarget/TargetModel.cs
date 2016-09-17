using System.ComponentModel.DataAnnotations;

namespace InjectDisplayAttribute.ExampleTarget
{
    public class TargetModel
    {
        public string PropertyToGetAnAttribute { get; set; }

        public string PropertyThatHasNoTranslation { get; set; }

        [Display]
        public string PropertyToLeaveAsIs { get; set; }

        public class TargetDto
        {
            public string PropertyToGetAnAttribute { get; set; }
        }
    }
}