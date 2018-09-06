export class Page<T> {
    public data: T[];
    public total: number;
    public currentPage: number;
    public pageSize: number;
    public pagesCount: number;
    public totalCount?: number;
}
