﻿using LW.Client.Models;

namespace LW.Client.Services
{
    public interface IAccountManagement
    {
        public Task<FormResult> RegisterAsync(string email,  string password);
        public Task<FormResult> LoginAsync(string email,  string password);
        public Task LogoutAsync();
        public Task<bool> CheckAuthenticatedAsync();
    }
}
