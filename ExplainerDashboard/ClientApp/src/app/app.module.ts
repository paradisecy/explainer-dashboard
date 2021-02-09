import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { DxDataGridModule } from 'devextreme-angular/ui/data-grid';
import { DxBoxModule } from 'devextreme-angular/ui/box';
import { DxTextAreaModule } from 'devextreme-angular/ui/text-area';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatCardModule } from '@angular/material/card/';
import { DxSelectBoxModule } from 'devextreme-angular/ui/select-box';
import { DxButtonModule } from 'devextreme-angular/ui/button';
import { DxPopupModule } from 'devextreme-angular/ui/popup';
import { DxFormModule } from 'devextreme-angular';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    DxDataGridModule,
    DxBoxModule,
    MatCardModule,
    DxTextAreaModule,
    DxFormModule,
    DxPopupModule,
    FormsModule,
    DxSelectBoxModule,
    DxButtonModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
    ]),
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
