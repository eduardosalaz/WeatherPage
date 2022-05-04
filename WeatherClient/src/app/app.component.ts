import { Component } from '@angular/core';
import {LatLng, latLng, LeafletMouseEvent, tileLayer} from "leaflet";
import {HttpClient} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],

})
export class AppComponent {

  constructor(private httpClient: HttpClient) { }

  title = 'WeatherClient';
  options = {
    layers: [
      tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {maxZoom: 18, attribution: '...'})
    ],
    center: latLng(46.879966, -121.726909),
    zoom: 3
  };
  zoom: any;
  center: any;
  lat: number | undefined;
  long: number | undefined;
  metric = true;
  temp = undefined;
  presion = undefined;
  humedad = undefined;
  pais = undefined;
  units: string | undefined;
  ciudad = undefined;

  handleEvent(click: string, $event: LeafletMouseEvent) {
    this.lat = $event.latlng.lat;
    this.long = $event.latlng.lng;
  }

  onZoomChange($event: number) {

  }

  onCenterChange($event: LatLng) {
    //console.log($event)
  }

  // @ts-ignore
  submitCoords() {
    if (this.lat === undefined) {
      alert("Selecciona un lugar en el mapa");
    } else {
      let apiUrl = "http://localhost:5092";
      let requestUrl = `/weather_data/${this.lat}/${this.long}/${this.metric}`
      if (!this.metric){
        this.units = "Fahrenheit";
      }else{
        this.units = "Celsius";
      }
      this.httpClient.get(apiUrl+requestUrl).subscribe(data =>{
        // @ts-ignore
        this.temp = data.main.temp;
        // @ts-ignore
        this.presion = data.main.pressure;
        // @ts-ignore
        this.humedad = data.main.humidity;
        // @ts-ignore
        this.pais = data.sys.country;
        // @ts-ignore
        this.ciudad = data.name;
      })
    }
  }

  onChange(value: any) {
    if (value === "Fahrenheit") {
      this.metric = false;

    } else {
      this.metric = true;
    }
  }
}
