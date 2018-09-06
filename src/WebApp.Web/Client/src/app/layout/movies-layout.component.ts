import { Component } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';

@Component({
    selector: 'movies-layout',
    templateUrl: './movies-layout.component.html',
    styleUrls: ['./movies-layout.component.scss']
})
export class MoviesLayoutComponent {
    public menuOpened = true;
    public menuMode = MenuMode.side;

    constructor(
        private breakpointObserver: BreakpointObserver) {
        this.breakpointObserver.observe(['(min-width: 1200px)'])
            .subscribe(state => {
                this.menuOpened = state.matches;
                this.menuMode = state.matches ? MenuMode.side : MenuMode.over;
            });
    }

    public openMenu() {
        this.menuOpened = !this.menuOpened;
    }
}

export enum MenuMode {
    side = 'side',
    push = 'push',
    over = 'over'
}
