namespace Rovia.UI.Automation.ScenarioObjects
{
    public abstract class Passenger
    {
        public bool Equals(Passenger other)
        {
            return string.Equals(FirstName, other.FirstName) && string.Equals(MiddleName, other.MiddleName) && string.Equals(LastName, other.LastName) && string.Equals(BirthDate, other.BirthDate) && string.Equals(Emailid, other.Emailid) && string.Equals(Gender, other.Gender);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MiddleName != null ? MiddleName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (BirthDate != null ? BirthDate.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Emailid != null ? Emailid.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Gender != null ? Gender.GetHashCode() : 0);
                return hashCode;
            }
        }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string Emailid { get; set; }
        public string Gender { get; set; }

    }
}
