namespace FormularPortal.Core.Models
{
    public abstract class FormElement : ICloneable
    {
        public string Name { get; set; } = string.Empty;

        public bool IsRequired { get; set; }

        public object Clone()
        {
            object tmp = this.MemberwiseClone();

            return tmp;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
