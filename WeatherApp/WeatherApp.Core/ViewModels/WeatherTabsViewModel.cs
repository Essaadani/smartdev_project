﻿using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Core.Messages;
using WeatherApp.Core.Models;
using WeatherApp.Core.Services;

namespace WeatherApp.Core.ViewModels
{
    public class WeatherTabsViewModel : MvxViewModel
    {
        //WEER (EERSTE TWEE PAGINA'S)
        private Weather _weather;
        public Weather Weather
        {
            get { return _weather; }
            set
            {
                _weather = value;
                RaisePropertyChanged(() => Weather);
            }
        }

        //LIST (LAATSTE PAGINA)
        private List<Weather.Daily.DailyDatas> _dailyDataList;
        public List<Weather.Daily.DailyDatas> DailyDataList
        {
            get { return _dailyDataList; }
            set
            {
                _dailyDataList = value;
                RaisePropertyChanged(() => DailyDataList);
            }
        }

        //Voor bericht met de locatie
        private readonly IMvxMessenger _messenger;
        private readonly MvxSubscriptionToken _token;

        private readonly IMvxNavigationService _navigationService;
        protected readonly IWeatherService _weatherService;

        //create a lazy instance for each viewmodel of the tabs in the tabview
        private readonly Lazy<WeatherViewModel> _weatherViewModel;
        private readonly Lazy<TabDetailsViewModel> _tabDetailsViewModel;
        private readonly Lazy<TabWeekTableViewModel> _tabWeekTableViewModel;

        //property to access value of the lazy instance
        public WeatherViewModel WeatherVM => _weatherViewModel.Value;
        public TabDetailsViewModel TabDetailsVM => _tabDetailsViewModel.Value;
        public TabWeekTableViewModel TabWeekVM => _tabWeekTableViewModel.Value;

        public WeatherTabsViewModel(IWeatherService weatherService, IMvxNavigationService navigationService, IMvxMessenger messenger)
        {
            //Subcriben aan de locatiemessage
            _messenger = messenger;
            _token = messenger.Subscribe<LocationMessage>(OnLocationMessage);

            //OnLocationMessage();

            //Navigeren
            _navigationService = navigationService;

            //Weer service
            this._weatherService = weatherService;           

            //initialize lazy instance via ioc construct
            _weatherViewModel = new Lazy<WeatherViewModel>(Mvx.IocConstruct<WeatherViewModel>);
            _tabDetailsViewModel = new Lazy<TabDetailsViewModel>(Mvx.IocConstruct<TabDetailsViewModel>);
            _tabWeekTableViewModel = new Lazy<TabWeekTableViewModel>(Mvx.IocConstruct<TabWeekTableViewModel>);

        }

        //Voor de tests
        public WeatherTabsViewModel(IWeatherService weatherService)
        {
            this._weatherService = weatherService;
            /*
            GlobalVariables._LATITUDE = 50;
            GlobalVariables._LONGITUDE = 3;
            GetWeatherData();
            */
        }

        private void OnLocationMessage(LocationMessage message)
        {
            GlobalVariables._LATITUDE = message.Latitude;
            GlobalVariables._LONGITUDE = message.Longitude;
            GetWeatherData();
        }

        //Weather data opvragen + opvullen
        public async void GetWeatherData()
        {
            Weather = await _weatherService.GetWeather();
            DailyDataList = await _weatherService.GetDailyDatas();

            WeatherVM.Weather = this.Weather;
            TabDetailsVM.Weather = this.Weather;
            TabWeekVM.DailyDataList = this.DailyDataList;
        }

        public IMvxCommand NotificationsCommand
        {
            get
            {
                return new MvxCommand(Notifications);
            }
        }

        public void Notifications()
        {
            _navigationService.Navigate<NotificationsViewModel>();
        }
    }
}
