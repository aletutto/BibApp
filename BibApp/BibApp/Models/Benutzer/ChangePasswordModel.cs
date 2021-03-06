﻿using System.ComponentModel.DataAnnotations;

namespace BibApp.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Das Aktuelle Passwort-Feld ist erforderlich.")]
        [DataType(DataType.Password)]
        [Display(Name = "Aktuelles Passwort")]
        public string OldPassword { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}", ErrorMessage = "Passwort muss enthalten: Mindestens 8 Zeichen, 1 Großbuchstaben, 1 Kleinbuchstaben, 1 Ziffer und 1 Sonderzeichen")]
        [Required(ErrorMessage = "Das Neues Passwort-Feld ist erforderlich.")]
        [StringLength(100, ErrorMessage = "Das {0} muss mindestens {2} und maximal {1} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Passwort")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Das Passwort bestätigen-Feld ist erforderlich.")]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("NewPassword", ErrorMessage = "Das Passwort und die Passwort-Bestätigung stimmen nicht überein.")]
        public string ConfirmPassword { get; set; }

    }
}
