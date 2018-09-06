import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MoviesComponent } from 'app/components/movies/movies.component';
import { MovieDetailsComponent } from 'app/components/movie-details/movie-details.component';

const routes: Routes = [
    {
        path: '',
        component: MoviesComponent
    },
    {
        path: ':studio/recent/:page',
        component: MoviesComponent
    },
    {
        path: 'search/:studio/:searchQuery/:page',
        component: MoviesComponent
    },
    {
        path: 'watch/:studio/:movie',
        component: MovieDetailsComponent
    },
    {
        path: '**',
        redirectTo: ''
    }
];

@NgModule({
    imports: [
        RouterModule.forRoot(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class MoviesRoutingModule { }
