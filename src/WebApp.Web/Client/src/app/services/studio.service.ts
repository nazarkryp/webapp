import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { Store } from '@ngrx/store';
import * as fromRoot from '../movies/infrastructure/state/reducer';

import { WebApiService } from 'app/core/communication';
import { MovieAction } from 'app/movies/infrastructure/state';
import { Studio } from 'app/models/view';
import { StudioResponse } from '../models/response';
import { StudioMapper } from 'app/mapping';

@Injectable({
    providedIn: 'root'
})
export class StudioService {
    constructor(
        private store: Store<fromRoot.MovieState>,
        private webApiService: WebApiService,
        private studioMapper: StudioMapper) { }

    public getStudios(): Observable<Studio[]> {
        return this.webApiService.get<StudioResponse[]>('v1/studios')
            .pipe(map(studios => {
                return this.studioMapper.mapFromResponseArray(studios);
            }));
    }

    public setCurrentStudio(studio: Studio) {
        this.store.dispatch({
            type: MovieAction.SET_STUDIO,
            payload: studio
        });
    }
}
