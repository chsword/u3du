/*
 ** 2014 September 22
 **
 ** The author disclaims copyright to this source code.  In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */
 

/**
 *
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 * @param <T>
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class Node<T> : Collection<T> where T:Node<T>{
    
    private T parent;
    private List<T> children = new List<T>();
    
    public T getParent() {
        return parent;
    }
    
    protected void setParent(T parent) {
        this.parent = parent;
    }
    
    private void setChildrenParent(Collection<T> col, Node parent) {
        for (T child : col) {
            child.setParent(parent);
        }
    }
    
 
    public int size() {
        return children.size();
    }

 
    public bool isEmpty() {
        return children.isEmpty();
    }
     
    public bool contains(object o) {
        return children.contains(o);
    }

        public override  Iterator<T> iterator() {
        return new IteratorImpl(children.iterator());
    }

        public override  object[] toArray() {
        return children.toArray();
    }

        public override  <T> T[] toArray(T[] a) {
        return children.toArray(a);
    }

        public override  bool add(T e) {
        e.setParent(this);
        return children.add(e);
    }

        public override  bool remove(object o) {
        bool r = children.remove(o);
        if (r && o instanceof Node) {
            ((Node) o).setParent(null);
        }
        return r;
    }

        public override  bool containsAll(Collection<?> c) {
        return children.containsAll(c);
    }

        public override  bool addAll(Collection<? : T> c) {
        bool b = children.addAll(c);
        setChildrenParent(children, this);
        return b;
    }

    public bool addAll(int index, Collection<? : T> c) {
        bool b = children.addAll(index, c);
        setChildrenParent(children, this);
        return b;
    }

        public override  bool removeAll(Collection<?> c) {
        List<T> childrenRemoved = new ArrayList<>(children);
        bool b = children.removeAll(c);
        childrenRemoved.removeAll(children);
        setChildrenParent(childrenRemoved, null);
        return b;
    }

        public override  bool retainAll(Collection<?> c) {
        List<T> childrenRemoved = new ArrayList<>(children);
        bool b = children.retainAll(c);
        childrenRemoved.removeAll(children);
        setChildrenParent(childrenRemoved, null);
        return b;
    }

        public override  void clear() {
        setChildrenParent(children, null);
        children.clear();
    }

        public override  int GetHashCode() {
        return children.GetHashCode();
    }

        public override  bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (GetType() != obj.GetType()) {
            return false;
        }
         Node<?> other = (Node<?>) obj;
        if (!Objects.Equals(this.children, other.children)) {
            return false;
        }
        return true;
    }
    
    private class IteratorImpl : Iterator<T> {
        
        private  Iterator<T> proxy;
        private T current;
        
        private IteratorImpl(Iterator<T> proxy) {
            this.proxy = proxy;
        }

        @Override
        public bool hasNext() {
            return proxy.hasNext();
        }

        @Override
        public T next() {
            current = proxy.next();
            return current;
        }

        @Override
        public void remove() {
            proxy.remove();
            if (current != null) {
                current.setParent(null);
            }
        }
    }
}
