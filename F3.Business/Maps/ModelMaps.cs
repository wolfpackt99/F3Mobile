using AutoMapper;
using F3.ViewModels.Calendar;
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
            Mapper.Initialize(cfg => {

                cfg.CreateMap<ViewModels.WorkoutViewModel, CalenderViewModel>();
            });
        }
    }
}
