import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import Home from './components/Home';
import FetchData from './components/FetchData';
import Counter from './components/Counter';
import CatImageCollector from './components/CatImageCollector';
import CatTinder from './components/CatTinder';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/catImageCollector' component={ CatImageCollector } />
    <Route path='/catTinder' component={ CatTinder } />
    <Route path='/counter' component={ Counter } />
    <Route path='/fetchdata/:startDateIndex?' component={ FetchData } />
</Layout>;
