codeunit 50149 ItemAttributes
{
    procedure SetItemAttribute(ItemNumber: Text; AttributeName: Text; AttributeValue: Text)
    var
        Attribute: Record "Item Attribute";
        Val: Record "Item Attribute Value";
        Map: Record "Item Attribute Value Mapping";
        Id: Integer;

    begin
        Clear(Attribute);
        Attribute.SetRange(Attribute.Name, AttributeName);
        if Attribute.FindFirst() then begin
            Clear(Map);
            Map.SetRange("No.", ItemNumber);
            Map.SetRange("Item Attribute ID", Attribute.ID);
            if Map.FindFirst() then begin
                Id := Map."Item Attribute Value ID";
                Map.Delete();

                Clear(Val);
                Val.SetRange(Val."Attribute ID", Attribute.ID);
                Val.SetRange(Val.ID, Id);
                if Val.FindFirst() then begin
                    Val.Delete();
                end;
            end
        end;

        Clear(Val);
        Val.Init();
        Val."Attribute ID" := Attribute.ID;
        Val.Value := AttributeValue;
        Val.Blocked := false;
        Val.Insert();

        Clear(Map);
        Map.Init();
        Map."No." := ItemNumber;
        Map."Item Attribute ID" := Attribute.ID;
        Map."Item Attribute Value ID" := Val.ID;
        Map."Table ID" := 27;
        Map.Insert();
    end;

    procedure GetItemAttributeValue(ItemNumber: Text; AttributeName: Text): Text
    var
        Attribute: Record "Item Attribute";
        Val: Record "Item Attribute Value";
        Map: Record "Item Attribute Value Mapping";
        Result: Text;

    begin
        Clear(Result);

        Attribute.SetRange(Attribute.Name, AttributeName);
        if Attribute.FindFirst() then begin
            Clear(Map);
            Map.SetRange("No.", ItemNumber);
            Map.SetRange("Item Attribute ID", Attribute.ID);
            if Map.FindFirst() then begin
                Clear(Val);
                Val.SetRange(Val."Attribute ID", Attribute.ID);
                Val.SetRange(Val.ID, Map."Item Attribute Value ID");
                if Val.FindFirst() then begin
                    Result := Val.Value;
                end;
            end
        end;

        exit(Result);
    end;

    procedure GetItemAttributes(ItemNumber: Text): Text
    var
        Attribute: Record "Item Attribute";
        Val: Record "Item Attribute Value";
        Map: Record "Item Attribute Value Mapping";
        JsonObj: JsonObject;
        Results: JsonArray;
        Result: Text;

    begin
        Clear(Result);
        Clear(Results);

        if Attribute.FindSet() then begin
            repeat
                Clear(Map);
                Map.SetRange("No.", ItemNumber);
                Map.SetRange("Item Attribute ID", Attribute.ID);
                if Map.FindFirst() then begin
                    Clear(Val);
                    Val.SetRange(Val."Attribute ID", Attribute.ID);
                    Val.SetRange(Val.ID, Map."Item Attribute Value ID");
                    if Val.FindFirst() then begin
                        Clear(JsonObj);
                        JsonObj.Add('itemNumber', ItemNumber);
                        JsonObj.Add('attribute', Attribute.Name);
                        JsonObj.Add('value', Val.Value);
                        Results.Add(JsonObj);
                    end;
                end;
            until Attribute.Next() = 0;
        end;

        Results.WriteTo(Result);
        exit(Result);
    end;

    procedure GetAllItemAttributes(): Text
    var
        Item: Record Item;
        Attribute: Record "Item Attribute";
        Map: Record "Item Attribute Value Mapping";
        Val: Record "Item Attribute Value";
        JsonObj: JsonObject;
        Results: JsonArray;
        Result: Text;
    begin
        Clear(Result);
        Clear(Results);

        if Attribute.FindSet() then begin
            repeat
                if Item.FindSet() then begin
                    repeat
                        Clear(Map);
                        Map.SetRange("No.", Item."No.");
                        Map.SetRange("Item Attribute ID", Attribute.ID);
                        if Map.FindFirst() then begin
                            Clear(Val);
                            Val.SetRange(Val."Attribute ID", Attribute.ID);
                            Val.SetRange(Val.ID, Map."Item Attribute Value ID");
                            if Val.FindFirst() then begin
                                Clear(JsonObj);
                                JsonObj.Add('itemNumber', Item."No.");
                                JsonObj.Add('attribute', Attribute.Name);
                                JsonObj.Add('value', Val.Value);
                                Results.Add(JsonObj);
                            end;
                        end;
                    until Item.Next() = 0;
                end;
            until Attribute.Next() = 0;
        end;

        Results.WriteTo(Result);
        exit(Result);
    end;
}

codeunit 50150 ItemRecordLinks
{
    var
        LastLinkID: Integer;

    procedure GetLinkValue(ItemNumber: Text; Description: Text): Text
    var
        Item: Record Item;
        RecordLink: Record "Record Link";
        Result: Text;

    begin
        Item.Get(ItemNumber);

        RecordLink.SetRange("Record ID", Item.RecordId());
        RecordLink.SetRange("Description", Description);
        RecordLink.SetRange("Type", RecordLink.Type::Link);

        Clear(Result);

        if RecordLink.FindFirst() then begin
            Result := RecordLink."URL1";
        end;

        exit(Result);
    end;

    procedure GetLinks(ItemNumber: Text): Text
    var
        Item: Record Item;
        RecordLink: Record "Record Link";
        Result: Text;
        Results: JsonArray;
        Link: JsonObject;

    begin
        Item.Get(ItemNumber);

        RecordLink.SetRange("Record ID", Item.RecordId());
        RecordLink.SetRange("Type", RecordLink.Type::Link);

        Clear(Results);

        if RecordLink.FindSet() then begin
            repeat
                Clear(Link);
                Link.Add('itemNumber', ItemNumber);
                Link.Add('url', RecordLink."URL1");
                Link.Add('description', RecordLink.Description);
                Results.Add(Link);
            until RecordLink.Next() = 0;
        end;

        Results.WriteTo(Result);
        exit(Result);
    end;

    procedure GetAllLinks(): Text
    var
        Item: Record Item;
        RecordLink: Record "Record Link";
        Result: Text;
        Results: JsonArray;
        Link: JsonObject;

    begin
        Clear(Results);

        if Item.FindSet() then begin
            repeat
                Clear(RecordLink);

                RecordLink.SetRange("Record ID", Item.RecordId());
                RecordLink.SetRange("Type", RecordLink.Type::Link);

                if RecordLink.FindSet() then begin
                    repeat
                        Clear(Link);
                        Link.Add('itemNumber', Item."No.");
                        Link.Add('url', RecordLink."URL1");
                        Link.Add('description', RecordLink.Description);
                        Results.Add(Link);
                    until RecordLink.Next() = 0;
                end;
            until Item.Next() = 0;
        end;

        Results.WriteTo(Result);
        exit(Result);
    end;

    procedure SetLink(ItemNumber: Text; Description: Text; Url: Text)
    var
        Item: Record Item;
        RecordLink: Record "Record Link";
    begin
        Item.Get(ItemNumber);

        RecordLink.SetRange("Record ID", Item.RecordId());
        RecordLink.SetRange("Description", Description);
        RecordLink.SetRange("Type", RecordLink.Type::Link);

        if RecordLink.FindFirst() then begin
            RecordLink."URL1" := Url;
            RecordLink."User ID" := UserId;
            RecordLink.Modify();
        end
        else begin

            LastLinkID := 0;
            GetLastLinkID();
            LastLinkID += 1;

            RecordLink.Init();
            RecordLink."Link ID" := LastLinkId;
            RecordLink."Record ID" := Item.RecordId();
            RecordLink.Type := RecordLink.Type::Link;
            RecordLink.Company := CompanyName;
            RecordLink.Created := CurrentDateTime;
            RecordLink."User ID" := UserId;
            RecordLink.Description := Description;
            RecordLink."URL1" := Url;
            RecordLink.Insert();
        end;
    end;

    local procedure GetLastLinkID()
    var
        RecordLink: Record "Record Link";
    begin
        RecordLink.Reset();
        if RecordLink.FindLast() then
            LastLinkID := RecordLink."Link ID"
        else
            LastLinkID := 0;
    end;
}