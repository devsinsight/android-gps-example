using Android.App;
using Android.OS;
using Android.Widget;
using AndroidGPSExample.Activities;
using Autofac;
using System;
using XLabs.Ioc;
using XLabs.Ioc.Autofac;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Geolocation;

namespace AndroidGPSExample
{
    [Activity(Label = "Android GPS Example", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            
            global::Xamarin.Forms.Forms.Init(this, bundle);

            SetContainer();
            SetMenu();
           
        }

        public void SetContainer() {
            var builder = new ContainerBuilder();
            builder.Register(c => AndroidDevice.CurrentDevice).As<IDevice>();
            builder.RegisterType<Geolocator>().AsSelf();

            Resolver.SetResolver(new AutofacResolver(builder.Build()));
        }

        private void SetMenu() {
            Button mapButton = FindViewById<Button>(Resource.Id.MapButton);
            mapButton.Click += Button_Click;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(MapActivity)));
        }
    }
}

