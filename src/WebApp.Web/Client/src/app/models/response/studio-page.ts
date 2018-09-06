import { Page } from '../common';
import { MovieResponse } from './movie';
import { StudioResponse } from './studio';

export class StudioPageResponse extends Page<MovieResponse> {
    public studio: StudioResponse;
}
