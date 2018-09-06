import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { MovieService } from 'app/services';
import { environment } from 'environments/environment';
import { Store } from '@ngrx/store';
import { MovieState } from 'app/movies/infrastructure/state';

@Component({
    selector: 'movies-movie-details',
    templateUrl: './movie-details.component.html',
    styleUrls: ['./movie-details.component.scss']
})
export class MovieDetailsComponent implements OnInit {
    public movie: any;
    public directUri: any;
    public studio: string;

    constructor(
        private route: ActivatedRoute,
        private movieService: MovieService) { }

    public tags = ['stockings', 'heels', 'sexy', 'legs', 'nudestockings', 'highheels', 'legs', 'beforesex'];

    public ngOnInit() {
        // this.store.pipe(select(getCurrentStudio));

        this.route.paramMap.subscribe(params => {
            this.studio = params.get('studio');
            const movie = params.get('movie');

            this.movieService.getMovie(this.studio, movie)
                .subscribe(movieDetails => {
                    this.movie = movieDetails;
                    if (environment.baseAddress.includes('localhost')) {
                        this.directUri = this.movie.directUri;
                    } else {
                        this.directUri = `${environment.baseAddress}v1/movies/stream?url={{movie.directUri}}`;
                    }
                });
        });
    }
}
