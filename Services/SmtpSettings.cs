﻿namespace BlazorOfficina.Services
{
    public class SmtpSettings
    {
        public string Host { get; set; } = null!;   // oppure aggiungi ? e gestisci null
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string From { get; set; } = null!;
    }
}
