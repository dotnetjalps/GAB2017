﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using WeatherBot.Helper;
using WeatherBot.Model;

namespace WeatherBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //var city = activity.Text ?? string.Empty;

            var query = activity.Text ?? string.Empty;
            var luisInformaion = await LuisHelper.ParseTextAsync(query);
            var city = GetCityName(luisInformaion);


            var weatherInforamion = await OpenWeatherAPIHelper.GetWeatherDataAsync(city);
            var weatherstring = GetWeather(weatherInforamion);
            await context.PostAsync(weatherstring);

            context.Wait(MessageReceivedAsync);
        }

        private static string GetWeather(WeatherInformation weatherInformation)
        {
            var weatherStringBuilder = new System.Text.StringBuilder();

            weatherStringBuilder.AppendLine($"Following is weather forcast for {weatherInformation.name}\r\n");
            weatherStringBuilder.AppendLine($"Current temperature is:  {weatherInformation.main.temp} Fahrenheit\r\n");
            weatherStringBuilder.AppendLine($"Minimum Tempearature will be:  {weatherInformation.main.temp_min} Fahrenheit\r\n");
            weatherStringBuilder.AppendLine($"Maximum Tempearature will be:  {weatherInformation.main.temp_max} Fahrenheit\r\n");
            weatherStringBuilder.AppendLine($"Wind Speed will be:  {weatherInformation.wind.speed} Miles/hr\r\n");
            weatherStringBuilder.AppendLine($"Humidity will be:  {weatherInformation.main.humidity} percent\r\n");

            return weatherStringBuilder.ToString();
        }

        private static string GetCityName(LuisInformation luisInformation)
        {
            var cityName = string.Empty;

            if (luisInformation != null && luisInformation.intents.Count > 0)
            {
                switch (luisInformation.intents[0].intent.ToLower())
                {
                    case "weather":
                        cityName = luisInformation.entities[0].entity;
                        break;
                }
            }
            return cityName;
        }
    }
}