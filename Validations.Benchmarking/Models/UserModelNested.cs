namespace Validations.Benchmarking.Models
{
    public class UserModelNested : UserModel
    {
        public AddressModel[] Addresses { get; set; } = [];
    }
}
