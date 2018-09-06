import { Page } from '../common';
import { Movie } from './movie';
import { Studio } from './studio';

export class StudioPage extends Page<Movie> {
    public studio: Studio;
}
