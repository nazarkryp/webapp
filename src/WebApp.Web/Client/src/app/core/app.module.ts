import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';

import { MoviesMaterialModule, MoviesRoutingModule } from 'app/core';

import { MoviesLayoutComponent } from 'app/layout/movies-layout.component';
import { HeaderComponent } from 'app/components/header/header.component';

import { StudioListComponent } from '../components/studio-list/studio-list.component';

import { StoreModule } from '@ngrx/store';

import { MoviesModule } from './movies.module';


@NgModule({
    declarations: [
        MoviesLayoutComponent,
        HeaderComponent,
        StudioListComponent
    ],
    imports: [
        BrowserModule,
        MoviesRoutingModule,
        BrowserAnimationsModule,
        FormsModule,
        ReactiveFormsModule,
        MoviesMaterialModule,
        HttpClientModule,
        FlexLayoutModule,
        MoviesModule,
        StoreModule.forRoot({})
    ],
    providers: [],
    bootstrap: [MoviesLayoutComponent]
})
export class AppModule { }
