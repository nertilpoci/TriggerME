import { Injectable } from '@angular/core';
import { APP_CONFIG, AppConfig  } from '../../shared/app.config';
import {Subscription,BehaviorSubject} from 'rxjs';
@Injectable()
export class LoadingService  {
   public busy:BehaviorSubject<Subscription> = new BehaviorSubject<Subscription>(null);
   public message:BehaviorSubject<string> = new BehaviorSubject<string>('Loading Data. Please wait');
   
    constructor( ) { }
    loader(state,message ='Loading Data. Please wait')
    {
        this.message.next(message);
        this.busy.next(state);
    }
   
}



