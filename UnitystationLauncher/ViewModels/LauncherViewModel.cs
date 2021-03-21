﻿using ReactiveUI;
using UnitystationLauncher.Models;
using System.Reactive;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Serilog;
using System.Reactive.Linq;

namespace UnitystationLauncher.ViewModels
{
    public class LauncherViewModel : ViewModelBase
    {
        private readonly AuthManager authManager;
        private readonly Lazy<LoginViewModel> logoutVM;
        private readonly Lazy<HubUpdateViewModel> hubUpdateVM;
        private readonly Config config;
        string username;
        PanelBase[] panels;
        ViewModelBase? selectedPanel;

        private readonly HttpClient http;

        public string Username
        {
            get => username;
            set => this.RaiseAndSetIfChanged(ref username, value);
        }

        public PanelBase[] Panels
        {
            get => panels;
            set => this.RaiseAndSetIfChanged(ref panels, value);
        }

        public ViewModelBase? SelectedPanel
        {
            get => selectedPanel;
            set => this.RaiseAndSetIfChanged(ref selectedPanel, value);
        }

        public ReactiveCommand<Unit, LoginViewModel> Logout { get; }
        public ReactiveCommand<Unit, HubUpdateViewModel> ShowUpdateReqd { get; }

        public ReactiveCommand<Unit, Unit> Refresh { get; }

        public LauncherViewModel(
            AuthManager authManager,
            ServersPanelViewModel serversPanel,
            InstallationsPanelViewModel installationsPanel,
            Lazy<LoginViewModel> logoutVM,
            HttpClient http,
            Lazy<HubUpdateViewModel> hubUpdateVM, 
            NewsPanelViewModel news,
            SettingsPanelViewModel settings,
            Config config)
        {
            this.authManager = authManager;
            this.logoutVM = logoutVM;
            this.hubUpdateVM = hubUpdateVM;
            this.config = config;
            this.http = http;
            Panels = new PanelBase[]
            {
                news,
                serversPanel,
                installationsPanel,
                settings
            };
            Username = this.authManager!.AuthLink.User.DisplayName;
            Logout = ReactiveCommand.Create(LogoutImp);
            Refresh = ReactiveCommand.Create(serversPanel.OnRefresh, null);
            ShowUpdateReqd = ReactiveCommand.Create(ShowUpdateImp);
            SelectedPanel = serversPanel;

            ValidateClientVersion();
        }


        async Task ValidateClientVersion()
        {
            var clientConfig = await config.GetServerHubClientConfig();
            if (clientConfig.BuildNumber != Config.CurrentBuild)
            {
                Log.Information($"Client is old ({Config.CurrentBuild}) new version is ({clientConfig.BuildNumber})");
                Observable.Return(Unit.Default).InvokeCommand(ShowUpdateReqd);
            }
        }

        LoginViewModel LogoutImp()
        {
            authManager.SignOutUser();
            File.WriteAllText(Path.Combine(Path.Combine(Config.RootFolder, "prefs.json")), JsonConvert.SerializeObject(new Prefs()));
            File.Delete(Path.Combine(Config.RootFolder, "settings.json"));
            return logoutVM.Value;
        }

        HubUpdateViewModel ShowUpdateImp()
        {
            return hubUpdateVM.Value;
        }
    }
}
