namespace DomainModel.overload {
    public static class ExtensionMethods {
        public static int getAge(this DateTime dateOfBirth) {
            // Calculate the age.
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year; 

            // check if birthday is another time this year
            if (dateOfBirth.Date > today.AddYears(-age)) age--;

            //return age
            return age;
        }
    }
}
