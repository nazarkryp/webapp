import { Component, OnInit, Inject, ViewEncapsulation, ViewChild } from '@angular/core';
import { Movie } from 'app/models/view';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

import { MovieService } from 'app/services';
import { Store } from '@ngrx/store';
import { MovieState } from 'app/movies/infrastructure/state/reducer';

import * as fromRoot from 'app/movies/infrastructure/state/reducer';
import { PromptService } from 'app/services/prompt.service';
import { Prompt } from '../../../models/common';
import { map, filter } from 'rxjs/operators';
import { Observable, merge } from 'rxjs';

@Component({
    selector: 'movies-movie-dialog',
    templateUrl: './movie-dialog.component.html',
    styleUrls: ['./movie-dialog.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class MovieDialogComponent implements OnInit {
    @ViewChild('video')
    private video: any;

    constructor(
        private store: Store<MovieState>,
        private dialogRef: MatDialogRef<MovieDialogComponent>,
        private movieService: MovieService,
        private promptService: PromptService,
        @Inject(MAT_DIALOG_DATA) public movie: Movie) {
        this.dialogRef.disableClose = true;
    }

    public play() {
        if (this.video.nativeElement.paused) {
            this.video.nativeElement.play();
        } else {
            this.video.nativeElement.pause();
        }
    }

    public ngOnInit() {
        // this.store.select(fromRoot.getCurrentStudio)
        //     .subscribe(studio => {
        //         this.movieService.getMovie(studio.id, this.movie.objectId)
        //             .subscribe(movie => {

        //             });
        //     });

        const escapeEvents = this.dialogRef.keydownEvents().pipe(filter(e => e.key === 'Escape'));

        merge(escapeEvents, this.dialogRef.backdropClick())
            .subscribe(() => {
                const prompt = new Prompt('Closing video', 'Are you sure you want to close video?');
                const paused = !this.video.nativeElement.paused;
                this.video.nativeElement.pause();

                this.promptService.prompt(prompt)
                    .subscribe((result) => {
                        if (result) {
                            this.dialogRef.close();
                        } else if (paused) {
                            this.video.nativeElement.play();
                        }
                    });
            });
    }
}
