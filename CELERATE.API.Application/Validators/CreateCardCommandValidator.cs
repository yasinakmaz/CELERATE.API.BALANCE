using FluentValidation;
using CELERATE.API.Application.Commands;

namespace CELERATE.API.Application.Validators
{
    public class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
    {
        public CreateCardCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Ad Soyad boş olamaz");
            RuleFor(x => x.TcIdentityNumber).NotEmpty().Length(11).WithMessage("TC Kimlik No 11 haneli olmalıdır");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası boş olamaz");
            RuleFor(x => x.Gender).NotEmpty().WithMessage("Cinsiyet seçimi gereklidir");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Yaş sıfırdan büyük olmalıdır");
            RuleFor(x => x.UserType).NotEmpty().WithMessage("Kullanıcı tipi boş olamaz");
            RuleFor(x => x.NfcCardId).NotEmpty().WithMessage("NFC Kart ID boş olamaz");
            RuleFor(x => x.BranchId).NotEmpty().WithMessage("Şube seçimi gereklidir");
        }
    }
}