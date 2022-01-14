import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppService {

  apiUrl: any = (window as any).location.port === '9001' ? '/api' :  'http://localhost:9002';

  constructor(private httpClient: HttpClient) {
  }

  WeatherForecast(): Observable<any[]> {
    return this.httpClient.get<any[]>(
      `${this.apiUrl}/weatherforecast`
    );
  }
}