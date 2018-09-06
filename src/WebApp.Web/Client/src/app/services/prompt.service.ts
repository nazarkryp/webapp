import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material';

import { Observable } from 'rxjs';
import { Prompt } from 'app/models/common';

import { PromptComponent } from 'app/components/shared/prompt/prompt.component';

@Injectable({
    providedIn: 'root'
})
export class PromptService {
    constructor(
        private dialog: MatDialog) { }

    public prompt(prompt: Prompt): Observable<any> {
        return this.dialog.open(PromptComponent, {
            width: '420px',
            autoFocus: false,
            data: prompt,
            panelClass: 'prompt-dialog-container'
        }).afterClosed();
    }
}
