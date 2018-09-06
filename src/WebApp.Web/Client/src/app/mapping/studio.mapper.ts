import { Injectable } from '@angular/core';

import { AttachmentResponse, StudioResponse } from 'app/models/response';
import { Attachment, Studio } from 'app/models/view';
import { IMapper } from './mapper';

@Injectable({
    providedIn: 'root'
})
export class StudioMapper implements IMapper<StudioResponse, Studio> {
    public mapFromResponse(response: StudioResponse): Studio {
        const studio = new Studio();

        studio.id = response.id;
        studio.name = response.name;
        studio.objectId = response.objectId;

        return studio;
    }

    public mapFromResponseArray(moviesResponse: StudioResponse[]): Studio[] {
        return moviesResponse.map(response => this.mapFromResponse(response));
    }
}
