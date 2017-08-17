import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'savecat',
    templateUrl: './savecat.component.html'
})

export class SaveCatComponent {
    constructor(private http: Http) {
    }
    
    submitted = false;
    onSubmit() { this.submitted = true; }
    
    newCat() {
        let randomnumber = Math.floor(Math.random() * (999 - 1 + 1)) + 1;        
        let cat : ICat = { id: randomnumber, dateOfBirth: '1/1/11', name: 'Mew' };
        return cat;    
    }

    public saveCat() {
        this.http.post('/api/cats', this.newCat()).subscribe(resp => {
            console.log(resp);
        });
    }
}

interface ICat {
    id: number;
    dateOfBirth: string;
    name: string;
}

 //model = new Hero(18, 'Dr IQ', this.powers[0], 'Chuck Overstreet');
 //submitted = false;