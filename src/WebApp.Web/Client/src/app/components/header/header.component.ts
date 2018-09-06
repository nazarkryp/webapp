import { Component, OnInit, ViewChild, ElementRef, Output } from '@angular/core';

import { MovieService } from 'app/services';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router, ActivationEnd } from '@angular/router';
import { EventEmitter } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Store } from '@ngrx/store';
import { MovieState, getCurrentStudio } from '../../movies/infrastructure/state';
import { select } from '@ngrx/store';
import { Studio } from '../../models/view';

@Component({
    selector: 'movies-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
    private studio: Studio;

    public showSearchBar: boolean;
    public mobile: boolean;

    public formGroup: FormGroup;
    @ViewChild('searchInput')
    public searchInput: ElementRef<any>;

    @Output('menuOpened')
    public menuOpened = new EventEmitter();

    public get searchQuery(): FormControl {
        return this.formGroup.get('searchQuery') as FormControl;
    }

    constructor(
        private router: Router,
        private builder: FormBuilder,
        private breakpointObserver: BreakpointObserver,
        private store: Store<MovieState>,
        private movieService: MovieService) {
        this.formGroup = this.builder.group({
            searchQuery: new FormControl('', Validators.compose([Validators.maxLength(50)]))
        });

        this.breakpointObserver.observe(['(max-width: 500px)'])
            .subscribe(state => {
                this.mobile = state.matches;
            });
    }

    public showMenu() {
        this.menuOpened.next();
    }

    public showSearch() {
        this.showSearchBar = !this.showSearchBar;

        if (this.showSearchBar) {
            this.searchInput.nativeElement.focus();
        }
    }

    public searchMovies() {
        const value = this.searchQuery.value;

        if (value) {
            this.router.navigate(['search', this.studio.id, value, 1]);
        }
    }

    public searchFocusLost() {
        if (!this.formGroup.get('searchQuery').value) {
            // this.showSearchBar = false;
        }
    }

    public ngOnInit() {
        this.router.events.subscribe(event => {
            if (event instanceof ActivationEnd) {
                const search = event.snapshot.paramMap.get('searchQuery');

                if (!search) {
                    this.formGroup.get('searchQuery').setValue('');
                    this.showSearchBar = false;
                } else {
                    this.formGroup.get('searchQuery').setValue(search);
                    this.showSearchBar = true;
                }
            }
        });

        this.store.pipe(select(getCurrentStudio))
            .subscribe((studio) => {
                this.studio = studio;
            });
    }
}
