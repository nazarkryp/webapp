import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { MovieMapper, StudioMapper } from 'app/mapping';
import { WebApiService } from 'app/core/communication';
import { StudioPageMapper } from '../mapping/studio-page.mapper';
import { StudioPageResponse } from '../models/response/studio-page';
import { Store } from '@ngrx/store';
import { MovieAction } from 'app/movies/infrastructure/state';
import { QueryFilter } from 'app/models/common/query-filter';
import { StudioPage } from '../models/view/studio-page';

@Injectable({
    providedIn: 'root'
})
export class MovieService {
    private readonly pageMapper: StudioPageMapper;

    constructor(
        private store: Store<any>,
        private webApiService: WebApiService,
        private movieMapper: MovieMapper,
        private studioMapper: StudioMapper) {
        this.pageMapper = new StudioPageMapper(this.movieMapper, this.studioMapper);
    }

    public getMovies(options: any): Observable<StudioPage> {
        const requestUri = this.buildRequestUri('v1/movies', options);

        return this.webApiService.get<StudioPageResponse>(requestUri)
            .pipe(
                map(response => this.pageMapper.map(response)),
                tap(movies => {
                    this.store.dispatch({
                        type: MovieAction.SET_MOVIES,
                        payload: movies
                    });
                }));
    }

    public getMovie(studio: string, movie: string): Observable<any> {
        return this.webApiService.get<any>(`v1/movies/${studio}/${movie}`);
    }

    private buildRequestUri(requestUri: string, options: QueryFilter) {
        if (!options) {
            return requestUri;
        }

        if (!options.page) {
            options.page = 1;
        }

        requestUri = `${requestUri}/${options.studio}?page=${options.page}`;

        if (options.search) {
            requestUri += `&search=${options.search}`;
        }

        return requestUri;
    }
}
