import { AttachmentResponse } from './attachment';
import { StudioResponse } from './studio';

export class MovieResponse {
    public id: number;
    public objectId: string;
    public title: string;
    public link: string;
    public date: Date;
    public attachments: AttachmentResponse[];
    public studio: StudioResponse;
    public duration: string;
}
