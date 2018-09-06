import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'movies-pagination',
    templateUrl: './pagination.component.html',
    styleUrls: ['./pagination.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaginationComponent {
    private _pagesCount: number;
    private range = 2;

    @Input() public currentPage: number;
    @Output() public changed = new EventEmitter<number>();

    public pages: number[];

    public get pagesCount(): number {
        return this._pagesCount;
    }

    @Input()
    public set pagesCount(value: number) {
        this._pagesCount = value;
        this.initializePages(value);
    }

    public showDots(page) {
        return (page === 2 && this.currentPage - page > this.range) || (page === this.pagesCount - 1 && page - this.currentPage > this.range);
    }

    public displayPage(page: number) {
        if (page === 1 || page === this.pagesCount) {
            return true;
        }

        if (this.currentPage < page) {
            return (page - this.currentPage) <= this.range;
        }

        if (this.currentPage > page) {
            return (this.currentPage - page) <= this.range;
        }

        return true;
    }

    public change(pageIndex: number) {
        this.changed.emit(pageIndex);
    }

    public previous() {
        this.changed.emit(this.currentPage - 1);
    }
    public next() {
        this.changed.emit(this.currentPage + 1);
    }

    private initializePages(pagesCount: number) {
        this.pages = new Array(pagesCount);

        for (let i = 0; i < pagesCount; i++) {
            this.pages[i] = i + 1;
        }
    }
}
