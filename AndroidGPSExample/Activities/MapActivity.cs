using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Widget;
using AndroidGPSExample.Domain;
using AndroidGPSExample.Domain.Contracts;
using System;
using XLabs.Ioc;
using static Android.Gms.Common.Apis.GoogleApiClient;
using static Android.Gms.Maps.GoogleMap;

namespace AndroidGPSExample.Activities
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity,
        IOnMapReadyCallback,
        IOnMyLocationClickListener,
        IOnMyLocationButtonClickListener,
        IConnectionCallbacks,
        IOnConnectionFailedListener,
        Android.Gms.Location.ILocationListener
    {
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private GoogleApiClient _googleApiClient;
        private Marker _marker;
        private IGeoLocationService _geoLocationService;

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            _map.MyLocationEnabled = true;
            _map.SetOnMyLocationClickListener(this);
            _map.SetOnMyLocationButtonClickListener(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);

            InitMapFragment();
            SetCurrentPositionButton();
            SetStartListeningButton();
            SetStopListeningButton();
            _geoLocationService = Resolver.Resolve<IGeoLocationService>();

            _googleApiClient = new Builder(this, this, this)
                .AddApi(LocationServices.API).Build();

        }

        protected override void OnPause()
        {
            base.OnPause();

            if (_googleApiClient.IsConnected)
            {
                LocationServices.FusedLocationApi.RemoveLocationUpdates(_googleApiClient, this);
                _googleApiClient.Disconnect();
            }
        }

        private FusedLocationProviderClient GetLocationService()
        {
            return LocationServices.GetFusedLocationProviderClient(this);
        }

        private void InitMapFragment()
        {
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(MapTypeSatellite)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }
            _mapFragment.GetMapAsync(this);
        }

        private void SetCurrentPositionButton()
        {
            Button currentLocationButton = FindViewById<Button>(Resource.Id.currentLocationButton);
            currentLocationButton.Click += async (sender, e) =>
            {
                var locationService = GetLocationService();
                var location = await locationService.GetLastLocationAsync();
                var latlng = new LatLng(location.Latitude, location.Longitude);

                if (_marker == null)
                {
                    MarkerOptions markerOptions = new MarkerOptions();
                    markerOptions.SetPosition(latlng);
                    markerOptions.SetTitle("Home");
                    _marker = _map.AddMarker(markerOptions);
                }
                else
                {
                    _marker.Position = latlng;
                }

                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(latlng);
                builder.Zoom(18);
                builder.Bearing(155);
                builder.Tilt(65);
                CameraPosition cameraPosition = builder.Build();
                _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));

            };

        }

        private void SetStartListeningButton()
        {
            Button startButton = FindViewById<Button>(Resource.Id.startButton);
            startButton.Click += (sender, e) =>
            {
                if (_googleApiClient.IsConnected) return;

                _googleApiClient.Connect();
            };
        }

        private void SetStopListeningButton()
        {
            Button stopButton = FindViewById<Button>(Resource.Id.stopButton);
            stopButton.Click += async (sender, e) =>
            {
                if (!_googleApiClient.IsConnected) return;

                await LocationServices.FusedLocationApi.RemoveLocationUpdatesAsync(_googleApiClient, this);
                _googleApiClient.Disconnect();
            };
        }

        public void OnMyLocationClick(Location location)
        {
            //TODO: On Click Blue Dot
        }

        public bool OnMyLocationButtonClick()
        {
            //TODO: Cross Button
            return false;
        }

        public void OnConnected(Bundle connectionHint)
        {
            LocationRequest locRequest = new LocationRequest();

            locRequest.SetPriority(100);
            locRequest.SetFastestInterval(500);
            locRequest.SetInterval(1000);

            LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, locRequest, this);
        }

        public void OnLocationChanged(Location location)
        {
            Console.WriteLine("Latitude: {0}, Lngitude: {1}", location.Latitude, location.Longitude);
            _geoLocationService.SendGeoLocation(new GeoLocation(location.Latitude, location.Longitude));
        }

        public void OnConnectionSuspended(int cause)
        {
            //TODO: On Connection
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            //TODO: On Fail
        }

    }
}