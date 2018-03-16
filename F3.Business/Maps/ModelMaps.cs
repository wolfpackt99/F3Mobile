using AutoMapper;
using F3.ViewModels.Calendar;
using Google.Apis.Calendar.v3.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3.Business.Maps
{
    public static class ModelMaps
    {
        public static void InitMaps()
        {
            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<ViewModels.WorkoutViewModel, CalenderViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.CalendarID))
                .AfterMap((s, d) =>
                {
                    d.Location = $"{s.Address} {s.City} {s.State} { s.Zipcode}";
                    d.Description = JsonConvert.SerializeObject(
                        new
                        {
                            SiteQ = d.Q,
                            Meets = Enum.GetName(typeof(DayOfWeek), d.DayOfWeek),
                            d.LocationHint,
                            d.DisplayLocation,
                            d.Time,
                            d.Region
                        });
                });

                cfg.CreateMap<CalendarListEntry, CalenderViewModel>()
                    .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                    .ForMember(d => d.CalendarID, opt => opt.MapFrom(s => s.Id))
                    .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Summary))
                    .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                    //.ForMember(d => d.MetaData = meta,
                    .ForMember(d => d.Location, opt => opt.MapFrom(s => s.Location))
                    //.ForMember(d => d.Type = meta != null ? meta.type : "",
                    .ForMember(d => d.TimeZone, opt => opt.MapFrom(s => s.TimeZone))
                    .AfterMap((source, destination) =>
                    {
                        try
                        {
                            var meta = JsonConvert.DeserializeObject<MetaDataViewModel>(source.Description);
                            destination.MetaData = meta;
                            destination.Type = meta != null ? meta.type : "";
                            destination.Region = meta.Region;
                            destination.Q = meta.SiteQ;
                            destination.Time = meta.Time;
                            destination.Day = meta.Meets;
                            destination.Group = meta.Meets;
                            destination.DisplayLocation = meta.DisplayLocation;
                            destination.LocationHint = meta.LocationHint;
                        }
                        catch (Exception exp)
                        {
                            System.Diagnostics.Debug.WriteLine("----calendar------");
                            System.Diagnostics.Debug.WriteLine(source.Description);
                            System.Diagnostics.Debug.WriteLine(exp.Message);
                            System.Diagnostics.Debug.WriteLine("----endcalendar-----");
                        }
                    });

            });
        }
    }
}
