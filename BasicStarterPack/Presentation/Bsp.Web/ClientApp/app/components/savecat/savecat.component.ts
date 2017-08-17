import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'savecat',
    templateUrl: './savecat.component.html'
})

export class SaveCatComponent {
    constructor(private http: Http) {
    }

    public saveCat() {
        console.log('test save cat');
        let randomnumber = Math.floor(Math.random() * (999 - 1 + 1)) + 1;        
        let cat : ICat = { id: randomnumber, dateOfBirth: '1/1/11', name: 'Mew' };
        this.http.post('/api/cats', cat).subscribe(resp => {
            console.log(resp);
        });
    }
}

interface ICat {
    id: number;
    dateOfBirth: string;
    name: string;
}