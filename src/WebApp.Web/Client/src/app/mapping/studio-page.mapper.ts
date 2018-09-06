import { Movie, Studio } from '../models/view';
import { MovieResponse, StudioResponse } from '../models/response';
import { PageMapper } from './page.mapper';
import { StudioPage } from '../models/view/studio-page';
import { StudioPageResponse } from '../models/response/studio-page';
import { IMapper } from './mapper';

export class StudioPageMapper extends PageMapper<MovieResponse, Movie> {
    constructor(
        protected movieMapper: IMapper<MovieResponse, Movie>,
        protected studioMapper: IMapper<StudioResponse, Studio>) {
        super(movieMapper);
    }

    public map(response: StudioPageResponse): StudioPage {
        try {
            const page = new StudioPage();

            page.studio = this.studioMapper.mapFromResponse(response.studio);
            page.currentPage = response.currentPage;
            page.totalCount = response.totalCount;
            page.pageSize = response.pageSize;
            page.total = response.total;
            page.data = this.movieMapper.mapFromResponseArray(response.data);
            page.pagesCount = response.pagesCount;

            return page;
        } catch (err) {
            console.log(err);
            throw err;
        }
    }
}
