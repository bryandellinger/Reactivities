export interface Activity {
    id: string;
    title: string;
    date: Date | null;
    category: string;
    city: string;
    venue: string;
    description: string;
}