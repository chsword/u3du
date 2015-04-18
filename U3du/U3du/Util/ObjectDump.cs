/*
 ** 2013 November 25
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
 */
public class ObjectDump {

    private static  TostringStyle STYLE_NORMAL = new ObjectTostringStyle(false);
    private static  TostringStyle STYLE_RECURSIVE = new ObjectTostringStyle(true);
    
    public static string ToString(object obj) {
        return ToString(obj, true);
    }
    
    public static string ToString(object obj, bool recursive) {
        return ReflectionTostringBuilder.ToString(obj, recursive ? STYLE_RECURSIVE : STYLE_NORMAL);
    }
    
    private static class ObjectTostringStyle : TostringStyle {
        
        private static  string INDENT = "  ";
        
        private  stringBuffer indentBuffer = new stringBuffer();
        private  bool recursive;

        ObjectTostringStyle(bool recursive) {
            base();
            
            this.recursive = recursive;
            
            setUseIdentityHashCode(false);
            setUseShortClassName(true);
            setArrayStart("[");
            setArrayEnd("]");
            setContentStart(" {");
            setFieldSeparatorAtStart(true);
            setFieldNameValueSeparator(" = ");

            updateSeparators();
        }

        private void indent() {
            indentBuffer.append(INDENT);
            updateSeparators();
        }

        private void unindent() {
            int len = indentBuffer.length();
            if (indentBuffer.length() > 0) {
                indentBuffer.setLength(len - INDENT.length());
            }
            updateSeparators();
        }

        @Override
        protected void appendDetail(stringBuffer buffer, string fieldName, object value) {
            appendObject(buffer, value);
        }
        
        @Override
        protected void appendDetail(stringBuffer buffer, string fieldName, Collection<?> coll) {
            indent();
            buffer.append(getArrayStart());
            buffer.append(getFieldSeparator());
            
            Iterator<?> it = coll.iterator();
            while (it.hasNext()) {
                appendObject(buffer, it.next());
                
                if (it.hasNext()) {
                    buffer.append(getArraySeparator());
                    buffer.append(getFieldSeparator());
                }
            }

            unindent();
            buffer.append(getFieldSeparator());
            buffer.append(getArrayEnd());
        }
        
        @Override
        protected void appendDetail(stringBuffer buffer, string fieldName, Map<?, ?> map) {
            indent();
            buffer.append(getArrayStart());
            buffer.append(getFieldSeparator());
            
            Iterator<? : Map.Entry> it = map.entrySet().iterator();
            
            while (it.hasNext()) {
                Map.Entry entry = it.next();
                
                appendObject(buffer, entry.getKey());
                buffer.append(getFieldNameValueSeparator());
                appendObject(buffer, entry.getValue());
                
                if (it.hasNext()) {
                    buffer.append(getArraySeparator());
                    buffer.append(getFieldSeparator());
                }
            }

            unindent();
            buffer.append(getFieldSeparator());
            buffer.append(getArrayEnd());
        }
        
        private void appendObject(stringBuffer buffer, object obj) {
            if (recursive && hasDefaultTostring(obj)) {
                indent();
                buffer.append(ReflectionTostringBuilder.ToString(obj, this));
                unindent();
            } else {
                buffer.append(obj);
            }
        }
        
        private bool hasDefaultTostring(object obj) {
            try {
                // check if the declaring class of ToString() is java.lang.object
                return obj.GetType().getMethod("ToString").getDeclaringClass() == object.class;
            } catch (SecurityException ex) {
                // class with security manager?
                return false;
            } catch (NoSuchMethodException ex) {
                // wat
                return true;
            }
        }

        private void updateSeparators() {
            string indentGlobal = indentBuffer.ToString();
            setFieldSeparator(SystemUtils.LINE_SEPARATOR + indentGlobal + INDENT);
            setContentEnd(SystemUtils.LINE_SEPARATOR + indentGlobal + "}");
        }
    }

    private ObjectDump() {
    }
}
