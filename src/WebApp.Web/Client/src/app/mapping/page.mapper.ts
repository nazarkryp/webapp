import { IMapper } from './mapper';
import { Page } from 'app/models/common';

export class PageMapper<TSource, TResult> {
    constructor(
        protected movieMapper: IMapper<TSource, TResult>) { }

    public map(responsePage: Page<TSource>): Page<TResult> {
        try {
            const page = new Page<TResult>();

            page.currentPage = responsePage.currentPage;
            page.pageSize = responsePage.pageSize;
            page.total = responsePage.total;
            page.data = this.movieMapper.mapFromResponseArray(responsePage.data);
            page.pagesCount = responsePage.pagesCount;

            return page;
        } catch (err) {
            throw err;
        }
    }
}
