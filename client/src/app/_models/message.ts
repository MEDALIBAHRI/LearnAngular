export interface Messages {
    id: number;
    senderId: number;
    senderUserName: string;
    senderPhotoUrl: string;
    recipientId: number;
    recipientUserName: string;
    recipientPhotoUrl?: any;
    dateRead: Date;
    dateSent: Date;
    content: string;
}