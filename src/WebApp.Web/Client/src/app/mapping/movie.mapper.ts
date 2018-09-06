import { Injectable } from '@angular/core';

import { IMapper } from './mapper';
import { AttachmentMapper } from './attachment.mapper';
import { StudioMapper } from './studio.mapper';
import { MovieResponse } from 'app/models/response';
import { Movie } from 'app/models/view';

@Injectable({
    providedIn: 'root'
})
export class MovieMapper implements IMapper<MovieResponse, Movie> {
    constructor(
        private attachmentMapper: AttachmentMapper,
        private studioMapper: StudioMapper) { }

    public mapFromResponse(response: MovieResponse): Movie {
        const movie = new Movie();

        movie.id = response.id;
        movie.objectId = response.objectId;
        movie.date = response.date;
        movie.link = response.link;
        movie.duration = response.duration;

        if (response.studio) {
            movie.studio = this.studioMapper.mapFromResponse(response.studio);
        }

        movie.attachments = this.attachmentMapper.mapFromResponseArray(response.attachments);
        movie.title = response.title;

        return movie;
    }

    public mapFromResponseArray(moviesResponse: MovieResponse[]): Movie[] {
        return moviesResponse.map(response => this.mapFromResponse(response));
    }
}
