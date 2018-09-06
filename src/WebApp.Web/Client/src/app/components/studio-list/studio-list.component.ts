import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ActivationEnd } from '@angular/router';

import { Observable } from 'rxjs';

import { StudioService } from 'app/services/studio.service';
import { Studio } from 'app/models/view';

@Component({
    selector: 'movies-studio-list',
    templateUrl: './studio-list.component.html',
    styleUrls: ['./studio-list.component.scss']
})
export class StudioListComponent implements OnInit {
    public studios: Observable<Studio[]>;
    public activeStudio: Studio;

    constructor(
        private router: Router,
        private studioService: StudioService) { }

    public select(studio: Studio) {
        this.studioService.setCurrentStudio(studio);
        this.activeStudio = studio;
        this.router.navigate([studio.id, 'recent', 1]);
    }

    public ngOnInit() {
        let currentStudio: string = null;
        this.router.events.subscribe(event => {
            if (event instanceof ActivationEnd) {
                currentStudio = event.snapshot.paramMap.get('studio');
            }
        });

        this.studios = this.studioService.getStudios();

        this.studios.subscribe(studios => {
            let studio: Studio;
            if (currentStudio) {
                studio = studios.find(e => e.id === currentStudio);
            } else {
                studio = studios[studios.length - 1];
            }

            this.activeStudio = studio;
            this.studioService.setCurrentStudio(studio);

            if (!currentStudio) {
                this.router.navigate([studio.id, 'recent', 1]);
            }
        });
    }
}
