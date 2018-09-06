import { Injectable } from '@angular/core';

import { AttachmentResponse } from 'app/models/response';
import { Attachment } from 'app/models/view';
import { IMapper } from './mapper';

@Injectable({
    providedIn: 'root'
})
export class AttachmentMapper implements IMapper<AttachmentResponse, Attachment> {
    public mapFromResponse(response: AttachmentResponse): Attachment {
        const attachment = new Attachment();

        attachment.id = response.id;
        attachment.url = response.url;

        return attachment;
    }

    public mapFromResponseArray(moviesResponse: AttachmentResponse[]): Attachment[] {
        return moviesResponse.map(response => this.mapFromResponse(response));
    }
}
